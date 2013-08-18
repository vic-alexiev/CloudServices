using Spring.IO;
using Spring.Social.Dropbox.Api;
using Spring.Social.Dropbox.Connect;
using Spring.Social.OAuth1;
using System;
using System.Diagnostics;
using System.IO;

internal class DropboxPhotoAlbumPublisher
{
    private const string AppKey = "oov03puj0e1tx1n";
    private const string AppSecret = "zg8r9s7o44u6f48";
    private const string OAuthTokenFileName = "OAuthTokenFileName.txt";

    private static OAuthToken LoadOAuthToken()
    {
        string[] lines = File.ReadAllLines(OAuthTokenFileName);
        OAuthToken oauthAccessToken = new OAuthToken(lines[0], lines[1]);
        return oauthAccessToken;
    }

    private static void AuthorizeAppViaOAuth(DropboxServiceProvider dropboxServiceProvider)
    {
        // Authorization without callback url
        Console.Write("Getting request token...");
        OAuthToken oAuthToken = dropboxServiceProvider.OAuthOperations.FetchRequestTokenAsync(null, null).Result;
        Console.WriteLine("Done.");

        OAuth1Parameters parameters = new OAuth1Parameters();
        string authenticateUrl = dropboxServiceProvider.OAuthOperations.BuildAuthorizeUrl(
            oAuthToken.Value, parameters);
        Console.WriteLine("Redirect the user for authorization to {0}", authenticateUrl);
        Process.Start(authenticateUrl);
        Console.Write("Press [Enter] when authorization attempt has succeeded.");
        Console.ReadLine();

        Console.Write("Getting access token...");
        AuthorizedRequestToken requestToken = new AuthorizedRequestToken(oAuthToken, null);
        OAuthToken oauthAccessToken =
            dropboxServiceProvider.OAuthOperations.ExchangeForAccessTokenAsync(requestToken, null).Result;
        Console.WriteLine("Done.");

        string[] oAuthData = new string[] { oauthAccessToken.Value, oauthAccessToken.Secret };
        File.WriteAllLines(OAuthTokenFileName, oAuthData);
    }

    private static void Main()
    {
        DropboxServiceProvider dropboxServiceProvider =
                new DropboxServiceProvider(AppKey, AppSecret, AccessLevel.AppFolder);

        if (!File.Exists(OAuthTokenFileName))
        {
            AuthorizeAppViaOAuth(dropboxServiceProvider);
        }

        OAuthToken oAuthAccessToken = LoadOAuthToken();

        IDropbox dropbox = dropboxServiceProvider.GetApi(oAuthAccessToken.Value, oAuthAccessToken.Secret);

        Entry folderEntry = new Entry();
        string folderName = string.Empty;

        while (true)
        {
            try
            {
                Console.WriteLine("Folder name:");
                folderName = Console.ReadLine();
                folderEntry = dropbox.CreateFolderAsync(folderName).Result;
                Console.WriteLine("Folder '{0}' created.", folderEntry.Path);
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
            }
        }

        Console.WriteLine("Number of files to upload:");

        int filesCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < filesCount; i++)
        {
            Console.WriteLine("File path:");
            string filePath = Console.ReadLine().Trim();
            Entry uploadFileEntry = dropbox.UploadFileAsync(
                new FileResource(filePath), "/" + folderName + "/" + Path.GetFileName(filePath)).Result;
            Console.WriteLine("File '{0}' uploaded.", uploadFileEntry.Path);
        }

        DropboxLink sharedUrl = dropbox.GetShareableLinkAsync(folderEntry.Path).Result;
        Console.WriteLine(sharedUrl.Url);
        Process.Start(sharedUrl.Url);
    }
}

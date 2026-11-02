namespace TwinCAT.XAE.StartPage.UI.Services.RecentItems
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using CommunityToolkit.Mvvm.Messaging;

    using Microsoft.VisualStudio.Shell;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using TwinCAT.XAE.StartPage.Core;
    using TwinCAT.XAE.StartPage.UI.Interfaces.RecentItems;
    using TwinCAT.XAE.StartPage.UI.Models;
    using TwinCAT.XAE.StartPage.UI.ViewModels;

    internal class MruPrivateSettingsService : ServiceBase, IMruService
    {
        private const string _offlinePath = "/content/indexed/collection[@name='CodeContainers.Offline']";
        private const string _privateSettingsXml = @"\ApplicationPrivateSettings.xml";

        private string _settingsPath
            //YD: kind of clunky, but it works, for now
            => MainViewModel.Package?.UserDataPath.Replace("Roaming", "Local") + _privateSettingsXml;

        //---

        public MruPrivateSettingsService(IAsyncMethodService methodService, IMessenger messenger) : base(methodService, messenger)
        { }

        //---

        public Task<List<RecentItem>> GetItemsAsync()
            => LoadAsync();

        public Task<bool> PinItemAsync(string key)
            => SetPinnedAsync(key, true);

        public Task<bool> UnpinItemAsync(string key)
            => SetPinnedAsync(key, false);

        public async Task<bool> RemoveItemAsync(string key)
        {
            var items = await LoadAsync();
            if (items is null)
                return false;

            var item = items.FirstOrDefault(x => x.Key == key);
            if (item is null)
                return false;

            items.Remove(item);

            return await SaveAsync(items);
        }

        public async Task<bool> UpdatePathAsync(string key, string newValue)
        {
            var items = await LoadAsync();
            if (items is null)
                return false;

            var item = items.FirstOrDefault(x => x.Key == key);
            if (item is null)
                return false;

            item.Key = newValue;
            item.Value.LocalProperties.FullPath = newValue;

            return await SaveAsync(items);
        }

        public async Task<bool> SetLastAccessedAsync(string path, DateTime date)
        {
            var items = await LoadAsync();
            if (items is null)
                return false;

            var item = items.FirstOrDefault(x => x.Key == path);
            if (item is null)
                return false;

            item.Value.LastAccessed = date;

            return await SaveAsync(items);
        }

        //---

        private async Task<bool> SetPinnedAsync(string key, bool value)
        {
            var items = await LoadAsync();
            if (items is null)
                return false;

            var item = items.FirstOrDefault(x => x.Key == key);
            if (item is null)
                return false;

            item.Value.IsFavorite = value;

            return await SaveAsync(items);
        }

        private async Task<List<RecentItem>> LoadAsync()
        {
            var items = new List<RecentItem>();

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // a new shell profile has no settings file until the shell first writes it
            if (!File.Exists(_settingsPath))
                return items; // empty list

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(_settingsPath);

            using (var offline = xmlDocument.DocumentElement.SelectNodes(_offlinePath))
            {
                var codeContainers = offline?.Item(0)?.FirstChild;

                if (codeContainers is null)
                    return items; // empty list

                var text = codeContainers.InnerText;

                items = JArray.Parse(text).ToObject<List<RecentItem>>();
            }

            return items;
        }

        private async Task<bool> SaveAsync(List<RecentItem> items)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            if (!File.Exists(_settingsPath))
                return false;

            var xmlDocument = new XmlDocument();

            xmlDocument.Load(_settingsPath);

            using (var offline = xmlDocument.DocumentElement.SelectNodes(_offlinePath))
            {
                var codeContainers = offline?.Item(0)?.FirstChild;

                if (codeContainers is null)
                    return false;
                var recentItems = JsonConvert.SerializeObject(items);

                codeContainers.InnerText = recentItems;
            }

            xmlDocument.Save(_settingsPath);

            return true;
        }
    }
}

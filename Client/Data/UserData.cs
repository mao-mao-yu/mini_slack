using System;
using System.IO;
using Common.Log;
using Common;

namespace Client.Data
{
    public class UserData
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Password { get; set; }
        public List<int> FriendsList { get; set; }
        public List<int> BlockList { get; set; }
        public List<int> GroupList { get; set; }

        public static readonly object lockObj = new object();

        private readonly Dictionary<string, dynamic> _dataDict = new Dictionary<string, dynamic>();

        private static readonly string _currentDirectory = Directory.GetCurrentDirectory();

        private static readonly string _userDataPath = Path.Combine(_currentDirectory, "UserData");

        public UserData(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    _dataDict = Json.Load<Dictionary<string, object>>(File.ReadAllText(filePath));
                }
                catch (Exception e)
                {
                    Logger.FWARNING("Convert str to json error " + e.Message);
                }
            }
        }

        private void SetDict()
        {
            _dataDict.Add("username", UserName);
            _dataDict.Add("password", Password);
            _dataDict.Add("nickname", NickName);
            _dataDict.Add("friendslist", FriendsList);
            _dataDict.Add("blocklist", BlockList);
            _dataDict.Add("grouplist", GroupList);
        }

        public void WriteToFile()
        {
            if (!Directory.Exists(_userDataPath))
            {
                Directory.CreateDirectory(_userDataPath);
            }
            string fileName = UserName + ".json";
            string outputPath = Path.Combine(_userDataPath, fileName);
            if (_dataDict.Count != 6)
            {
                Logger.FWARNING($"Insufficient user data. DataDict's count is {_dataDict.Count}");
                return;
            }
            using (FileStream fileStream = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                // 获取文件锁定
                fileStream.Lock(0, fileStream.Length);

                // 创建写入器
                using (StreamWriter sw = new StreamWriter(fileStream))
                {
                    // 写入内容
                    _ = sw.WriteAsync(Json.Dump(_dataDict));
                }

                // 释放文件锁定
                fileStream.Unlock(0, fileStream.Length);
            }
        }

        public Dictionary<string, object> GetDict()
        {
            return _dataDict;
        }
    }
}

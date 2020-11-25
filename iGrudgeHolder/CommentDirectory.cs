using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iGrudgeHolder
{
    class CommentDirectory
    {
        private readonly Dictionary<int, string> _driverComments = new Dictionary<int, string>();

        public string Path { get; private set; }

        public CommentDirectory(string path)
        {
            Path = path;
            Directory.CreateDirectory(path);
            Reload();
        }

        private void Reload()
        {
            _driverComments.Clear();
            foreach (var file in Directory.GetFiles(Path, "*.txt"))
            {
                try
                {
                    var userId = Convert.ToInt32(System.IO.Path.GetFileNameWithoutExtension(file));
                    _driverComments[userId] = File.ReadAllText(file);
                }
                catch (Exception e)
                {
                    // ignored
                }
            }
        }

        public string this[int userId]
        {
            get => _driverComments.TryGetValue(userId, out var value) ? value : "";
            set
            {
                _driverComments[userId] = value;
                string file = System.IO.Path.Combine(Path, userId.ToString() + ".txt");
                File.WriteAllText(file, value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// Read the opb_data.json file into memory and makes its contents available for the Fetcher / Bot for reading/editing

namespace OnePieceBot
{
    public sealed class Data
    {
        // API
        // getter 
        public static Data Instance { get { return m_instance; } }
        public List<Int64> UserIds
        {
            get
            {
                // return a copy of the user ids to to prevent simultaneous access 
                m_mut.WaitOne();
                List<Int64> tmp = new List<Int64>(m_data.UserIDs);
                m_mut.ReleaseMutex();
                return tmp;
            }
        }
        // TODO: simultaneous access?
        public UInt16 Chapter { get { return m_data.Chapter; } }
        public UInt16 Episode { get { return m_data.Episode; } }
        public string Token { get { return m_data.Token; } }

        // methods
        public void AddUser(Int64 user)
        {
            m_mut.WaitOne();
            if (!m_data.UserIDs.Contains(user))
            {
                m_data.UserIDs.Add(user);
            }
            SaveData();
            m_mut.ReleaseMutex();
        }

        public void RemoveUser(Int64 user)
        {
            m_mut.WaitOne();
            m_data.UserIDs.Remove(user);
            SaveData();
            m_mut.ReleaseMutex();
        }

        public bool NextChapter(UInt16 foundChapter)
        {
            bool success = false;
            m_mut.WaitOne();
            if(m_data.Chapter == foundChapter)
            {
                m_data.Chapter += 1;
                success = true;
            }
            SaveData();
            m_mut.ReleaseMutex();
            return success;
        }

        public bool NextEpisode(UInt16 foundChapter)
        {
            bool success = false;
            m_mut.WaitOne();
            if(m_data.Episode == foundChapter)
            {
                m_data.Episode += 1;
                success = true;
            }
            SaveData();
            m_mut.ReleaseMutex();
            return success;
        }


        // INTERNAL 
        private void ReadData()
        {
            string json = System.IO.File.ReadAllText(m_data_path);
            m_data = JsonSerializer.Deserialize<DataJsonRepresentation>(json); // if this doesnt work program should crash 
        }

        private void SaveData()
        {
            string json = JsonSerializer.Serialize(m_data);
            System.IO.File.WriteAllText(m_data_path, json);
        }

        // members
        private static readonly Data m_instance = new Data();
        private static Mutex m_mut = new Mutex();
        private readonly string m_data_path;
        private DataJsonRepresentation m_data;

        internal sealed class DataJsonRepresentation
        {
            public DataJsonRepresentation()
            {
                UserIDs = new List<Int64>();
                Token = "";
            }
            public string Token { get; set; }
            public UInt16 Chapter { get; set; }
            public UInt16 Episode { get; set; }
            public List<Int64> UserIDs { get; set; }
        }

        // initialization
        private Data()
        {
            m_data_path = "opb_data.json";
            m_data = new DataJsonRepresentation();
            ReadData();
        }
    }
}

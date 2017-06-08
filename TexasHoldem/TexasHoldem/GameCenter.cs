﻿using System;
using System.Collections.Generic;
using System.Linq;
using TexasHoldem.Exceptions;
using TexasHoldem.Game;
using TexasHoldem.Loggers;
using TexasHoldem.Logics;
using TexasHoldem.Users;

namespace TexasHoldem
{
    public class GameCenter
    {
        private static void Main()
        {   
			DatabaseContext db = new DatabaseContext();

	        // Display all users from the database - for debugging
	        var query1 = from u in db.Users
		        orderby u.Username
		        select u;

	        Console.WriteLine("All users in the database:");
	        List<User> res = query1.ToList();
	        foreach (var item in res)
	        {
		        Console.WriteLine(item.Username);
	        }
			Console.WriteLine("Press any key to exit...");
	        Console.ReadKey();
        }

        

        private static GameCenter _instance;
        public List<Tuple<IUser, bool>> Users { get; }
        public List<IRoom> Rooms;
        public int ExpCriteria { get; }
        public int DefaultRank { get; private set; }

		public const int MinRank = 0;
        public const int MaxRank = 10;
        public const int MinCriteria = 5;
        public const int MaxCriteria = 20;

        public UserLogic UserLogic;

        public List<IUser> GetTopStat()
        {
            List<IUser> users = new List<IUser>();
            foreach(Tuple<IUser,Boolean> t in Users)
            {
                users.Add(t.Item1);
            }
            users.Sort(delegate (IUser x, IUser y)
            {
                if (x.Wins > y.Wins) return 1;
                else if (y.Wins > x.Wins) return -1;
                else return 0;
            });
            return users;
        }


        private GameCenter()
        {
            Rooms = new List<IRoom>();
            UserLogic = new UserLogic();
            Users = UserLogic.GetAllUsers();
            ExpCriteria = 10;
        }

        // Implementation according to the Singleton Pattern
        public static GameCenter GetGameCenter()
        {
            return _instance ?? (_instance = new GameCenter());
        }

        public void DeleteAllRooms()
        {
            Rooms.Clear();
        }

        public Room CreateRoom(string roomName, string username, string creator, GamePreferences gp)
        {
            var user = UserLogic.GetLoggedInUser(username, Users);
            if (user != null)
            {
                var p = new Player(creator, user);
                if (IsRoomExist(roomName))
                {
                    var err = new IllegalRoomNameException("ERROR in CreateRoom: room name already taken!");
                    Logger.Log(Severity.Error, err.Message);
                    throw err;
                }
                var newRoom = new Room(roomName, p, gp);
                Rooms.Add(newRoom);
                Logger.Log(Severity.Action, "Room " + newRoom.Name + " created successfully by " + creator + "!");
                return newRoom;
            }
            var e = new IllegalUsernameException("ERROR in CreateRoom: Username does not exist!");
            Logger.Log(Severity.Error, e.Message);
            throw e;
        }

        public IRoom GetRoom(string roomName)
        {
            IRoom roomFound = null;
            for (var i = 0; i < Rooms.Count && roomFound == null; i++)
            {
                if (Rooms[i].Name == roomName)
                    roomFound = Rooms[i];
            }
            if (roomFound != null)
            {
                return roomFound;
            }
            var e = new IllegalRoomNameException("Error in GetRoom: Room " + roomName + " doesn't exist!");
            Logger.Log(Severity.Error, e.Message);
            throw e;
        }

        public bool IsRoomExist(string roomName)
        {
            try
            {
                GetRoom(roomName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public IRoom AddUserToRoom(string username, string roomName, bool isSpectator, string playerName = "")
        {
            var room = GetRoom(roomName);
            var user = UserLogic.GetLoggedInUser(username, Users);

            if (room != null)
            {
                if (isSpectator)
                {
                    room.Spectate(user);
                }
                else
                { 
                    
                return room.AddPlayer(new Player(playerName, user));
                }
            }
            else
            {
                var e = new IllegalRoomNameException("Error in AddUserToRoom: Room " + roomName + " doesn't exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            return room;
        }

        public IRoom RemoveUserFromRoom(string username, string roomName, string playerName)
        {
            var room = GetRoom(roomName);
            //IUser user = UserLogic.GetLoggedInUser(username, Users);

            if (room != null)
            {
                room.ExitRoom(playerName);
				IUser user = Users.First(u => u.Item1.Username == username).Item1;
				UserLogic.UpdateUser(user); // update wins, chips, games played, league?
            }
            else
            {
                var e = new IllegalRoomNameException("Error in RemoveUserFromRoom: Room " + roomName + " doesn't exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            return room;
        }

        public void DeleteRoom(string roomName)
        {
            for (var i = 0; i < Rooms.Count; i++)
            {
                if (Rooms[i].Name == roomName)
                {
                    Rooms.RemoveAt(i);
                    Logger.Log(Severity.Action, "Room " + roomName + " deleted successfully!");
                    return;
                }
            }
            var e = new IllegalRoomNameException("Room " + roomName + " does not exist!");
            Logger.Log(Severity.Error, e.Message);
            throw e;
        }

        public List<IRoom> FindGames(List<Predicate<IRoom>> predicates)
        {
            var ans = new List<IRoom>();
            string roomsFound = "";
            foreach (var r in Rooms)
            {
                var toAdd = true;
                foreach (var p in predicates)
                {
                    if (!p.Invoke(r))
                    {
                        toAdd = false;
                    }
                }
                if (toAdd)
                {
                    ans.Add(r);
                }
            }

            if (ans.Count > 0)
            {
                for (var i = 0; i < ans.Count; i++)
                {
                    if (i < ans.Count - 1)
                    {
                        roomsFound = roomsFound + ans[i].Name + ", ";
                    }
                    else
                    {
                        roomsFound = roomsFound + ans[i].Name + ".";
                    }

                }
                Logger.Log(Severity.Action, "Found the following game rooms: " + roomsFound);

            }
            else
            {
                var e = new Exception("ERROR in FindGames: No game rooms found.");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }

            return ans;
        }

        public string GetReplayFilename(string roomName)
        {
            return GetRoom(roomName).GameReplay;
        }

        public List<string> GetMessages(string username, string roomName)
        {
            var list = UserLogic.GetUser(username, Users).Notifications.FindAll(pair => pair.Item1 == roomName);
            var ans = new List<string>();
            foreach (var pair in list)
            {
                ans.Add(pair.Item2);
            }
            return ans;
        }
    }
}
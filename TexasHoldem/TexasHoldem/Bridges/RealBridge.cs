﻿using System;
using System.Collections;
using System.Collections.Generic;
using TexasHoldem.Services;

namespace TexasHoldem.Bridges
{
    public class RealBridge:IBridge
    {
        private readonly UserManager _userManager;
        private readonly GameManager _gameManager;
        private readonly ReplayManager _replayManager;

        public RealBridge()
        {
            _userManager = new UserManager();
            _gameManager = new GameManager();
            _replayManager = new ReplayManager();
        }

        public bool Register(string userName, string pass)
        {
            try
            {
                _userManager.Register(userName, pass);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public bool IsUserExist(string userName)
        {
            try
            {
                _userManager.Login(userName, userName);
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Username does not exist!"))
                    return false;
            }
            return true;
        }

        public bool DeleteUser(string username, string password)
        {
            try
            {
                _userManager.DeleteUser(username, password);
            }
            catch (Exception)
            {
               return false;
            }
            return true;
        }

        public bool Login(string userName, string pass)
        {
            try
            {
                _userManager.Login(userName, pass);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool IsLoggedIn(string userName, string pass)
        {
            return _userManager.IsUserLoggedIn(userName, pass);
        }

        public bool LogOut(string userName)
        {
            try
            {
                _userManager.Logout(userName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EditUsername(string username, string newUsername)
        {
            try
            {
                _userManager.EditProfileUsername(username, newUsername);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EditPassword(string username, string newPass)
        {
            try
            {
                _userManager.EditProfilePassword(username, newPass);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool EditAvatar(string username, string newPath)
        {
            try
            {
                _userManager.EditProfileAvatarPath(username, newPath);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CreateNewGame(string gameName, string username, string creatorName)
        {
            try
            {
                _gameManager.CreateGame(gameName, username, creatorName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CreateNewGameWithPrefrences(string gameName, string username, string creatorName, string gameType, int buyInPolicy,
            int chipPolicy, int minBet, int minPlayers, int maxPlayer, bool spectating)
        {
            try
            {
                _gameManager.CreateGameWithPreferences(gameName, username, creatorName, gameType, buyInPolicy,
                    chipPolicy, minBet, minPlayers, maxPlayer, spectating);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool IsGameExist(string gameName)
        {
            return _gameManager.IsRoomExist(gameName);
        }


        public IList findGames(string username, RoomFilter filter)
        {
            try
            { 
                List<Room> tmp = gameManager.FindGames(username, filter);
                List<string> ans = new List<string>();
                foreach (Room r in tmp)
                {
                    ans.Add(r.name);
                }
                return ans;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public bool JoinGame(string username, string roomName, string playerName)
        {
            try
            {
                _gameManager.JoinGame(username, roomName, playerName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SpectateGame(string username, string roomName, string playerName)
        {
            try
            {
                _gameManager.SpectateGame(username, roomName, playerName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool LeaveGame(string username, string roomName, string playerName)
        {
            try
            {
                _gameManager.LeaveGame(username, roomName, playerName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public int GetRank(string userName)
        {
            return _userManager.GetRank(userName);
        }

        public bool RaiseInGame(int raiseamount, string gamename, string playername)
        {
            try
            {
                _gameManager.PlaceBet(gamename, playername, raiseamount);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool CallInGame(string gamename, string playername)
        {
            try
            {
                _gameManager.Call(gamename, playername);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool FoldInGame(string gameName, string playerName)
        {
            try
            {
                _gameManager.Fold(gameName, playerName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetExpCriteria(string username, int exp)
        {
            try
            {
                _gameManager.SetExpCriteria(username, exp);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetDefaultRank(string username, int rank)
        {
            try
            {
                _gameManager.SetDefaultRank(username, rank);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetUserLeague(string username, string usernameToSet, int rank)
        {
            try
            {
                _gameManager.SetUserLeague(username, usernameToSet, rank);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SaveTurn(string roomName, int turnNum)
        {
            try
            {
                _replayManager.SaveTurn(roomName, turnNum);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool RestartGameCenter()
        {
            return _gameManager.RestartGameCenter();
        }

        public bool StartGame(string roomName)
        {
            try
            {
                _gameManager.StartGame(roomName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SetBet(string roomName, string playerName, int bet)
        {
            try
            {
                _gameManager.PlaceBet(roomName,playerName,bet);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void SetUserRank(string legalUserName, int newrank)
        {
            _userManager.ChangeRank(legalUserName, newrank);
        }
    }
}

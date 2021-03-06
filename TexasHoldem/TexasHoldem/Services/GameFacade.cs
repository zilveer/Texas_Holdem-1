﻿using System;
using System.Collections;
using System.Collections.Generic;
using TexasHoldem.Game;
using TexasHoldem.Logics;
using TexasHoldem.Users;

namespace TexasHoldem.Services
{
    public class GameFacade
    {
        private readonly GameCenter _gameCenter;
        private readonly MessageLogic _messageLogic;
        private readonly UserLogic _userLogic;

        public GameFacade(string dbName = "TexasDatabase")
        {
            _gameCenter = GameCenter.GetGameCenter(dbName);
            _messageLogic = new MessageLogic(); // TODO change?
            _userLogic = _gameCenter.UserLogic;
        }

        public IUser WebLogin(string username, string password)
        {
            return _userLogic.WebLogin(username, password);
        }

        public IUser GetStat(string userName)
        {
            return _userLogic.GetStat(userName);
        }

        public List<IUser> GetTopStat(int kind)
        {
            return _gameCenter.GetTopStat(kind);
        }

        public IRoom RoomStatus(string roomName)
        {
            return _gameCenter.GetRoom(roomName);
        }

        public void SetLeagues()
        {
            _userLogic.SetLeagues(_gameCenter.Users);
        }

        public Room CreateGame(string gameName, string username, string creatorName) // UC 5
        {
            return _gameCenter.CreateRoom(gameName, username, creatorName, new GamePreferences());
        }

        public Room CreateGameWithPreferences(string gameName, string username, string creatorName, string gameType,
            int buyInPolicy, int chipPolicy, int minBet, int minPlayers, int maxPlayers, bool spectating)
        {
            var gp = new GamePreferences
            {
                GameType = (Gametype) Enum.Parse(typeof(Gametype), gameType),
                BuyInPolicy = buyInPolicy,
                ChipPolicy = chipPolicy,
                MinBet = minBet,
                MinPlayers = minPlayers,
                MaxPlayers = maxPlayers,
                Spectating = spectating
            };

            return _gameCenter.CreateRoom(gameName, username, creatorName, gp);
        }

        public bool IsRoomExist(string roomName)
        {
            return _gameCenter.IsRoomExist(roomName);
        }

        public IRoom JoinGame(string username, string roomName, string playerName) // UC 6
        {
            return _gameCenter.AddUserToRoom(username, roomName, false, playerName);
        }

        public IRoom SpectateGame(string username, string roomName) // UC 7
        {
            return _gameCenter.AddUserToRoom(username, roomName, true);
        }

        public IRoom SpectatorExit(string username, string roomName) // UC 7
        {
            return _gameCenter.GetRoom(roomName).ExitSpectator(_userLogic.GetUser(username, _gameCenter.Users));
        }

        public IRoom LeaveGame(string username, string roomName, string playerName) // UC 8
        {
            return _gameCenter.RemoveUserFromRoom(username, roomName, playerName);
        }

        public List<IRoom> FindGames(string username) // UC 11 (Finds any available game)
        {
            var context = _userLogic.GetUser(username, _gameCenter.Users);

            var predicates = new List<Predicate<IRoom>>();
            var r = new RoomFilter
            {
                SepctatingAllowed = true
            };

            if (context.League != -1 && r.LeagueOnly != null && r.LeagueOnly.Value)
                predicates.Add(room => room.League == context.League);
            if (r.PlayerName != null)
                predicates.Add(room => room.HasPlayer(r.PlayerName));
            if (r.PotSize != null)
                predicates.Add(room => room.Pot == r.PotSize.Value);
            if (r.GameType != null)
                predicates.Add(room => room.GamePreferences.GameType.ToString() == r.GameType);
            if (r.BuyInPolicy != null)
                predicates.Add(room => room.GamePreferences.BuyInPolicy == r.BuyInPolicy.Value);
            if (r.ChipPolicy != null)
                predicates.Add(room => room.GamePreferences.ChipPolicy == r.ChipPolicy.Value);
            if (r.MinBet != null)
                predicates.Add(room => room.GamePreferences.MinBet == r.MinBet.Value);
            if (r.MinPlayers != null)
                predicates.Add(room => room.GamePreferences.MinPlayers == r.MinPlayers.Value);
            if (r.MaxPlayers != null)
                predicates.Add(room => room.GamePreferences.MaxPlayers == r.MaxPlayers.Value);
            if (r.SepctatingAllowed != null)
                predicates.Add(room => room.GamePreferences.Spectating == r.SepctatingAllowed.Value);

            return _gameCenter.FindGames(predicates);
        }

        public List<IRoom> FindGamesWithFilter(string legalUserName, bool leagueOnly, string gameType, int buyInPolicy, int chipPolicy, int minBet, int minPlayers, bool sepctatingAllowed)
        {
            var context = _userLogic.GetUser(legalUserName, _gameCenter.Users);

            var predicates = new List<Predicate<IRoom>>();
            var r = new RoomFilter
            {
                PlayerName = legalUserName,
                LeagueOnly = leagueOnly,
                GameType = gameType,
                BuyInPolicy = buyInPolicy,
                ChipPolicy = chipPolicy,
                MinBet = minBet,
                MinPlayers = minPlayers,
                SepctatingAllowed = sepctatingAllowed
            };

            if (context.League != -1 && r.LeagueOnly != null && r.LeagueOnly.Value)
                predicates.Add(room => room.League == context.League);
            if (r.PlayerName != null)
                predicates.Add(room => room.HasPlayer(r.PlayerName));
            if (r.PotSize != null)
                predicates.Add(room => room.Pot == r.PotSize.Value);
            if (r.GameType != null)
                predicates.Add(room => room.GamePreferences.GameType.ToString() == r.GameType);
            if (r.BuyInPolicy != null)
                predicates.Add(room => room.GamePreferences.BuyInPolicy == r.BuyInPolicy.Value);
            if (r.ChipPolicy != null)
                predicates.Add(room => room.GamePreferences.ChipPolicy == r.ChipPolicy.Value);
            if (r.MinBet != null)
                predicates.Add(room => room.GamePreferences.MinBet == r.MinBet.Value);
            if (r.MinPlayers != null)
                predicates.Add(room => room.GamePreferences.MinPlayers == r.MinPlayers.Value);
            if (r.MaxPlayers != null)
                predicates.Add(room => room.GamePreferences.MaxPlayers == r.MaxPlayers.Value);
            if (r.SepctatingAllowed != null)
                predicates.Add(room => room.GamePreferences.Spectating == r.SepctatingAllowed.Value);

            return _gameCenter.FindGames(predicates);
        }

        public List<IRoom> FindGames(string username, RoomFilter r) // UC 11 (Finds any available game)
        {
            var context = _userLogic.GetUser(username, _gameCenter.Users);

            var predicates = new List<Predicate<IRoom>>();

            if (r.LeagueOnly != null && r.LeagueOnly.Value)
                predicates.Add(room => room.CanJoin(context));
            if (r.PlayerName != null)
                predicates.Add(room => room.HasPlayer(r.PlayerName));
            if (r.PotSize != null)
                predicates.Add(room => room.Pot == r.PotSize.Value);
            if (r.GameType != null)
                predicates.Add(room => room.GamePreferences.GameType.ToString() == r.GameType);
            if (r.BuyInPolicy != null)
                predicates.Add(room => room.GamePreferences.BuyInPolicy == r.BuyInPolicy.Value);
            if (r.ChipPolicy != null)
                predicates.Add(room => room.GamePreferences.ChipPolicy == r.ChipPolicy.Value);
            if (r.MinBet != null)
                predicates.Add(room => room.GamePreferences.MinBet == r.MinBet.Value);
            if (r.MinPlayers != null)
                predicates.Add(room => room.GamePreferences.MinPlayers == r.MinPlayers.Value);
            if (r.MaxPlayers != null)
                predicates.Add(room => room.GamePreferences.MaxPlayers == r.MaxPlayers.Value);
            if (r.SepctatingAllowed != null)
                predicates.Add(room => room.GamePreferences.Spectating == r.SepctatingAllowed.Value);

            return _gameCenter.FindGames(predicates);
        }

        public Room StartGame(string gameName) // UC 12
        {
            return _gameCenter.GetRoom(gameName).StartGame();
        }

        public Room PlaceBet(string gameName, string player, int bet) // UC 13
        {
            return _gameCenter.GetRoom(gameName).SetBet(_gameCenter.GetRoom(gameName).GetPlayer(player), bet, false);
        }

        public Room Fold(string room, string userName)
        {
            return _gameCenter.GetRoom(room).Fold(_gameCenter.GetRoom(room).GetPlayer(userName));
        }

        public Room Call(string room, string userName)
        {
            return _gameCenter.GetRoom(room).Call(_gameCenter.GetRoom(room).GetPlayer(userName));
        }

        public void CalcLeague()
        {
            _userLogic.SetLeagues(_gameCenter.Users);
        }

        public IRoom PlayerWhisper(string room, string playernameSender, string usernameReceiver, string message)
        {
            IUser reciever = null;
            IRoom roomObj = null;

            try
            {
                roomObj = _gameCenter.GetRoom(room);
                reciever = _gameCenter.GetRoom(room).GetSpectator(usernameReceiver);
                return _messageLogic.PlayerWhisper(message, roomObj.GetPlayer(playernameSender), reciever, roomObj);
            }
            catch
            {
                roomObj = _gameCenter.GetRoom(room);
                return _messageLogic.PlayerWhisper(message, roomObj.GetPlayer(playernameSender),
                    roomObj.GetPlayer(usernameReceiver).User, roomObj);
            }
        }

        public IRoom SpectatorWhisper(string room, string usernameSender, string usernameReceiver, string message)
        {
            IUser reciever = null;
            reciever = _gameCenter.GetRoom(room).GetSpectator(usernameReceiver);
            return _messageLogic.SpectatorWhisper(message, _gameCenter.GetRoom(room).GetSpectator(usernameSender),
                reciever, _gameCenter.GetRoom(room));
        }

        public IRoom PlayerSendMessage(string room, string playerNameSender, string message)
        {
            return _messageLogic.PlayerSendMessage(message, _gameCenter.GetRoom(room).GetPlayer(playerNameSender),
                _gameCenter.GetRoom(room));
        }

        public IRoom SpectatorsSendMessage(string room, string usernameSender, string message)
        {
            return _messageLogic.SpectatorsSendMessage(message, _userLogic.GetUser(usernameSender, _gameCenter.Users),
                _gameCenter.GetRoom(room));
        }

        public bool RestartGameCenter(bool deleteUsers)
        {
            try
            {
                _gameCenter.DeleteAllRooms();
                if (deleteUsers)
                    _userLogic.DeleteAllUsers(_gameCenter.Users);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
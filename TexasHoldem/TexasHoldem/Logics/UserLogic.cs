﻿using System;
using System.Collections.Generic;
using System.Linq;
using TexasHoldem.Exceptions;
using TexasHoldem.Loggers;
using TexasHoldem.Users;

namespace TexasHoldem.Logics
{
    public class UserLogic
    {
        public void SetLeagues(List<Tuple<IUser, bool>> users)
        {
            var tempUsers = users.Where(user => user.Item1.League != -1).ToList();
            double size = tempUsers.Count / 10;
            var leagueSize = (int)Math.Ceiling(size);
            if (leagueSize == 0) leagueSize = 1;
            var currentSize = 0;
            var league = 10;
            var usersNum = 0;
            var done = new List<IUser>();
            IUser current = null;
            var maxWins = -1;
            for (var i = 0; i < tempUsers.Count; i++)
            {
                foreach (var u in tempUsers)
                {
                    if (u.Item1.Wins > maxWins && !done.Contains(u.Item1))
                    {
                        maxWins = u.Item1.Wins;
                        current = u.Item1;
                    }
                }
                if (current != null)
                {
                    current.League = league;
                    currentSize++;
                    done.Add(current);
                }
                usersNum++;
                if ((currentSize == leagueSize && leagueSize >= 2) || (currentSize > leagueSize))
                {
                    currentSize = 0;
                    league--;
                }
                if (tempUsers.Count == usersNum + 1 && tempUsers.Count % 2 == 1)
                {
                    league++;
                }
                maxWins = -1;
            }
        }

        public void Register(string username, string password, List<Tuple<IUser, bool>> users)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    var e = new IllegalUsernameException("ERROR in Register: Username already exists!");
                    Logger.Log(Severity.Error, e.Message);
                    throw e;
                }
            }

            users.Add(new Tuple<IUser, bool>(new User(username, password, "default.png", "default@gmail.com", 5000), false));
            // USERNAME & PASSWORD CONFIRMED
            Logger.Log(Severity.Action, "Registration completed successfully!");
        }

        public IUser Login(string username, string password, List<Tuple<IUser, bool>> users)
        {
            int i;
            IUser user = null;
            for (i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    if (users[i].Item1.Password == password)
                    {
                        if (users[i].Item2)
                        {
                            var e = new IllegalUsernameException("ERROR in Login: This User is already logged in.");
                            Logger.Log(Severity.Error, e.Message);
                            throw e;
                        }
                        user = users[i].Item1;
                        users[i] = new Tuple<IUser, bool>(users[i].Item1, true);
                    }
                    else
                    {
                        var e = new IllegalPasswordException("ERROR in Login: Wrong password!");
                        Logger.Log(Severity.Error, e.Message);
                        throw e;
                    }
                }
            }

            if (user == null)
            {
                var e = new IllegalUsernameException("ERROR in Login: Username does not exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            Logger.Log(Severity.Action, username + " logged in successfully!");

            return user;

        }

        public void Logout(string username, List<Tuple<IUser, bool>> users)
        {
            int i;
            var exist = false;
            var UserRooms = GameCenter.GetGameCenter().Rooms.Where(r => r.IsInRoom(username)).ToList();
            if (UserRooms.Count > 0)
            {
                var e = new Exception("can't log out before leaving all games");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            for (i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    if (!users[i].Item2)
                    {
                        var e = new IllegalUsernameException("ERROR in Logout: User is already logged off.");
                        Logger.Log(Severity.Error, e.Message);
                        throw e;
                    }
                    exist = true;
                    users[i] = new Tuple<IUser, bool>(users[i].Item1, false);
                }
            }

            if (!exist)
            {
                var e = new IllegalUsernameException("ERROR in Logout: Username does not exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            Logger.Log(Severity.Action, username + " logged out successfully!");
        }

        public void DeleteAllUsers(List<Tuple<IUser, bool>> users)
        {
            users.Clear();
        }

        public IUser EditUser(string username, string newUserName, string newPassword, string newAvatarPath, string newEmail, List<Tuple<IUser, bool>> users)
        {
            IUser ans= null;
            var userExists = false;
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    ans = users[i].Item1;
                    userExists = true;
                    if (!users[i].Item2)
                    {
                        var e = new IllegalUsernameException("ERROR in Edit Profile: This User is not logged in.");
                        Logger.Log(Severity.Error, e.Message);
                        throw e;
                    }
                    try
                    {
                        if (newUserName != null)
                        {
                            for (var j = 0; j < users.Count; j++)
                            {
                                if (users[j].Item1.Username == newUserName)
                                {
                                    var e = new IllegalUsernameException("ERROR in Edit Profile: New username already exists!");
                                    Logger.Log(Severity.Error, e.Message);
                                    throw e;
                                }
                            }
                            users[i].Item1.Username = newUserName;
                        }
                        if (newPassword != null)
                            users[i].Item1.Password = newPassword;
                        if (newAvatarPath != null)
                            users[i].Item1.AvatarPath = newAvatarPath;
                        if (newEmail != null)
                            users[i].Item1.Email = newEmail;
                    }
                    catch (Exception)
                    {
                        var e = new Exception("ERROR in Edit Profile: Invalid new user details!");
                        Logger.Log(Severity.Error, e.Message);
                        throw e;
                    }
                }
            }

            if (!userExists)
            {
                var e = new IllegalUsernameException("ERROR in Login: Username does not exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            Logger.Log(Severity.Action, username + "'s profile edited successfully!");
            return ans;
        }

        public IUser GetLoggedInUser(string username,List<Tuple<IUser, bool>> users)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    if (!users[i].Item2)
                    {
                        var err = new IllegalUsernameException("ERROR in GetLoggedInUser: This User is not logged in.");
                        Logger.Log(Severity.Error, err.Message);
                        throw err;
                    }
                    return users[i].Item1;
                }
            }
            var e = new IllegalUsernameException("ERROR in Edit Profile: This user doesn't exist.");
            Logger.Log(Severity.Error, e.Message);
            throw e;
        }

        public IUser GetUser(string username, List<Tuple<IUser, bool>> users)
        {
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {

                    return users[i].Item1;
                }
            }
            var e = new IllegalUsernameException("ERROR in GetUser: This User doesn't exist.");
            Logger.Log(Severity.Error, e.Message);
            throw e;
        }

        public void DeleteUser(string username, string password, List<Tuple<IUser, bool>> users)
        {
            Tuple<IUser, bool> userToDelete = null;
            for (var i = 0; i < users.Count; i++)
            {
                if (users[i].Item1.Username == username)
                {
                    if (users[i].Item1.Username == password)
                    {
                        userToDelete = users[i];
                    }
                    else
                    {
                        var e = new IllegalPasswordException("ERROR in Edit Profile: Wrong password!");
                        Logger.Log(Severity.Error, e.Message);
                        throw e;
                    }
                }
            }

            if (userToDelete == null)
            {
                var e = new IllegalUsernameException("ERROR in Login: Username does not exist!");
                Logger.Log(Severity.Error, e.Message);
                throw e;
            }
            users.Remove(userToDelete);
            Logger.Log(Severity.Action, "User: " + username + " deleted successfully!");
        }
    }
}

using System.Collections.Generic;
using TexasHoldem.Logics;
using TexasHoldem.Users;

namespace TexasHoldem.Game
{
    public interface IRoom
    {
        string CurrentWinners { get; set; }
        bool IsOn { get; set; }
        List<IUser> SpectateUsers { get; set; }
        List<IPlayer> Players { get; set; }
        Deck Deck { get; set; }
        Card[] CommunityCards { get; set; }
        string Name { get; set; }
        int League { get; set; }
        GamePreferences GamePreferences { get; set; }
        bool Flop { get; set; }
        int Pot { get; set; }
        GameStatus GameStatus { get; set; }
        HandLogic HandLogic { get; }
        int CurrentTurn { get; set; }
        bool HasPlayer(string name);
        Room AddPlayer(IPlayer p);
        void Spectate(IUser user);
        void DealTwo();
        void DealCommunityFirst();
        void DealCommunitySecond();
        void DealCommunityThird();
        bool IsInRoom(string name);
        bool Isspectator(string username);
        bool CanJoin(IUser user);
        Room StartGame();
        Room Call(IPlayer p);
        Room SetBet(IPlayer p, int bet, bool smallBlind);
        void ExitRoom(string player);
        List<IPlayer> Winners();
        void CalcWinnersChips(bool folded);
        void CleanGame();
        void NextTurn();
        IRoom ExitSpectator(IUser user);
        Room Fold(IPlayer p);
        IPlayer GetPlayer(string name);
        IUser GetSpectator(string name);
    }
}
using System.Collections.Generic;

namespace LUP
{
    [System.Serializable]
    public struct DeveloperInfo
    {
        public string role;
        public string name;

        public DeveloperInfo(string role, string name)
        {
            this.role = role;
            this.name = name;
        }
    }

    [System.Serializable]
    public struct GameTeam
    {
        public string gameName;
        public string gameCode;
        public List<DeveloperInfo> members;

        public GameTeam(string gameName, string gameCode)
        {
            this.gameName = gameName;
            this.gameCode = gameCode;
            this.members = new List<DeveloperInfo>();
        }

        public void AddMember(string role, string name)
        {
            members.Add(new DeveloperInfo(role, name));
        }
    }

    public class CreditData
    {
        public string projectTitle = "PROJECT LUP";

        private List<GameTeam> teams = new List<GameTeam>();
        private List<DeveloperInfo> commonTeam = new List<DeveloperInfo>();

        public CreditData()
        {
            InitializeCredits();
        }

        private void InitializeCredits()
        {

            GameTeam rlTeam = new GameTeam("Roguelike", "RL");
            rlTeam.AddMember("Game Developer", "Lee Juhyeong");
            rlTeam.AddMember("Game Developer", "Lee Dohyeon");
            teams.Add(rlTeam);


            GameTeam stTeam = new GameTeam("Shooting", "ST");
            stTeam.AddMember("Game Developer", "Lim Seongmin");
            stTeam.AddMember("Game Developer", "Yoo Joonsang");
            teams.Add(stTeam);


            GameTeam esTeam = new GameTeam("Extraction Shooter", "ES");
            esTeam.AddMember("Game Developer", "Kim Kisoo");
            esTeam.AddMember("Game Developer", "Kim Soohyeok");
            teams.Add(esTeam);


            GameTeam dsgTeam = new GameTeam("Deck Strategy Game", "DSG");
            dsgTeam.AddMember("Game Developer", "Seong Jegyeong");
            dsgTeam.AddMember("Game Developer", "Lee Dogyeong");
            dsgTeam.AddMember("Game Developer", "Park Joohyeon");
            teams.Add(dsgTeam);


            GameTeam pcrTeam = new GameTeam("Production & Construction", "PCR");
            pcrTeam.AddMember("Game Developer", "Jang Jooha");
            pcrTeam.AddMember("Game Developer", "Park Sieun");
            teams.Add(pcrTeam);


            commonTeam.Add(new DeveloperInfo("Game Developer", "Jeong Dohoon"));
            commonTeam.Add(new DeveloperInfo("Game Developer", "Kim Hyeonjoon"));
        }

        public List<GameTeam> GetTeams()
        {
            return teams;
        }

        public List<DeveloperInfo> GetCommonTeam()
        {
            return commonTeam;
        }

        public GameTeam? GetTeamByCode(string gameCode)
        {
            foreach (var team in teams)
            {
                if (team.gameCode == gameCode)
                {
                    return team;
                }
            }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TiperiBezTohoTiperi
{
    class Table
    {
        public String player;
        private List<Team> teams;
        public Table(String pl)
        {
            player = pl;
            teams = new List<Team>();
        }

        public void AddMatch(String home, String away, int scoreHome, int scoreAway)
        {
            if (!teams.Contains(new Team(home))) {
                var homeT = new Team(home);
                teams.Add(homeT);
            }

            if (!teams.Contains(new Team(away)))
            {
                var awayT = new Team(away);
                teams.Add(awayT);
            }

            if (scoreHome > scoreAway)
            {
                teams.Find(x => x.Equals(new Team(home))).wins++;
                teams.Find(x => x.Equals(new Team(away))).losses++;
            } else if (scoreAway > scoreHome)
            {
                teams.Find(x => x.Equals(new Team(home))).losses++;
                teams.Find(x => x.Equals(new Team(away))).wins++;
            } else
            {
                teams.Find(x => x.Equals(new Team(home))).draws++;
                teams.Find(x => x.Equals(new Team(away))).draws++;
            }

            teams.Find(x => x.Equals(new Team(home))).score[0] += scoreHome;
            teams.Find(x => x.Equals(new Team(home))).score[1] += scoreAway;
            teams.Find(x => x.Equals(new Team(away))).score[0] += scoreAway;
            teams.Find(x => x.Equals(new Team(away))).score[1] += scoreHome;

            teams.Find(x => x.Equals(new Team(home))).CalculatePoints();
            teams.Find(x => x.Equals(new Team(away))).CalculatePoints();
        }

        public void Sort()
        {
            teams.Sort();
        }

        public List<Team> GetTeams()
        {
            return this.teams;
        }

        public void ShowTable()
        {
            Console.WriteLine("{0, 15} {1, 6} {2, 6} {3, 6} {4, 6} {5, 7}", player, "Wins", "Draws", "Losses", "Points", "Score");
            foreach (Team t in teams)
            {
                Console.WriteLine("{0, 15} {1, 6} {2, 6} {3, 6} {4, 6} {5, 3}:{6, -3}", t.name, t.wins, t.draws, t.losses, t.points, t.score[0], t.score[1]);
            }
        }

    }

    class Team : IComparable
    {
        public String name;
        public int wins;
        public int draws;
        public int losses;
        public int[] score;
        public int points;

        public Team(String clubName)
        {
            name = clubName;
            score = new int[2] { 0, 0 };
            wins = 0;
            draws = 0;
            losses = 0;
            points = 0;
        }

        public void CalculatePoints()
        {
            points = wins * 3 + draws;
        }

        public int GetScoreDifference()
        {
            return score[0] - score[1];
        }

        public override bool Equals(object obj)
        {
            return this.name.Equals((obj as Team).name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public int CompareTo(object obj)
        {
            var other = obj as Team;
            if (this.points == other.points)
            {
                if (this.GetScoreDifference() == other.GetScoreDifference())
                {
                    if (this.score[0] == other.score[0])
                    {
                        Random rand = new Random();
                        return rand.Next(-5, 5);
                    }
                    else
                        return other.score[0] - this.score[0];
                }
                else
                    return other.GetScoreDifference() - this.GetScoreDifference();
            }
            else
                return other.points - this.points;
        }
    }
}

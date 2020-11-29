using System;
using System.Collections.Generic;
using System.Text;

namespace TiperiBezTohoTiperi
{
    class Match
    {
        private String teamHome { get; set; }
        private String teamAway { get; set; }
        private int[,] picks = new int[3, 2];
        private int[] result = new int[2];

        public Match (String h, String a, String pickA, String pickD, String pickP, String res)
        {
            teamHome = h;
            teamAway = a;
            picks[0, 0] = Int32.Parse(pickA.Split(':')[0]);
            picks[0, 1] = Int32.Parse(pickA.Split(':')[1]);
            picks[1, 0] = Int32.Parse(pickD.Split(':')[0]);
            picks[1, 1] = Int32.Parse(pickD.Split(':')[1]);
            picks[2, 0] = Int32.Parse(pickP.Split(':')[0]);
            picks[2, 1] = Int32.Parse(pickP.Split(':')[1]);
            result[0] = Int32.Parse(res.Split(':')[0]);
            result[1] = Int32.Parse(res.Split(':')[1]);
        }

        public Match(String h, String a, String pickA, String pickD, String pickP)
        {
            teamHome = h;
            teamAway = a;
            picks[0, 0] = Int32.Parse(pickA.Split(':')[0]);
            picks[0, 1] = Int32.Parse(pickA.Split(':')[1]);
            picks[1, 0] = Int32.Parse(pickD.Split(':')[0]);
            picks[1, 1] = Int32.Parse(pickD.Split(':')[1]);
            picks[2, 0] = Int32.Parse(pickP.Split(':')[0]);
            picks[2, 1] = Int32.Parse(pickP.Split(':')[1]);
            result[0] = 0;
            result[1] = 0;
        }

        public int[] GetPickById(int id)
        {
            var pickValue = new int[2];
            pickValue[0] = picks[id, 0];
            pickValue[1] = picks[id, 1];
            return pickValue;
        }
        public void SetPickValues(int id, int home, int away)
        {
            picks[id, 0] = home;
            picks[id, 1] = away;
        }
        public int[] GetResult()
        {
            return result;
        }
        public void SetResult(int home, int away)
        {
            result[0] = home;
            result[1] = away;
        }

        public String[] GetTeams()
        {
            var teams = new String[2];
            teams[0] = teamHome;
            teams[1] = teamAway;
            return teams;
        }

        public void SetTeams(String home, String away)
        {
            teamHome = home;
            teamAway = away;
        }

        public override string ToString()
        {

            return $"H: {teamHome} A: {teamAway} |A {picks[0,0]}:{picks[0,1]} |D {picks[1,0]}:{picks[1,1]} |P {picks[2,0]}:{picks[2,1]} |SC {result[0]}:{result[1]}";
        }

        public int[] Evaluate()
        {
            var points = new int[3];
            for (int i = 0; i < 3; i++)
            {
                // RULE 1, exact score - 10p
                if (picks[i, 0] == result[0] && picks[i, 1] == result[1])
                    points[i] = 10;
                // RULE 2, draw pick
                else if (picks[i, 0] == picks[i, 1] && result[0] == result[1])
                    points[i] = 7;

                else if ((picks[i, 0] > picks[i, 1] && result[0] > result[1]) || (picks[i, 0] < picks[i, 1] && result[0] < result[1]))
                {
                    // RULE 3, correct 1X2 and score of one team
                    if (picks[i, 0] == result[0] || picks[i, 1] == result[1])
                        points[i] = 5;
                    // RULE 4, correct 1X2
                    else
                        points[i] = 3;
                }
                // RULE 5, nothing
                else
                    points[i] = 0;

            }
            return points;
        }

    }
}

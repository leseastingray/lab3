﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Example Yahtzee website if you've never played
// https://cardgames.io/yahtzee/

namespace Yahtzee
{
    public partial class yahtzeeForm : Form
    {
        public yahtzeeForm()
        {
            InitializeComponent();
        }

        // you may find these helpful in manipulating the scorecard and in other places in your code
        private const int NONE = -1;
        private const int ONES = 0;
        private const int TWOS = 1;
        private const int THREES = 2;
        private const int FOURS = 3;
        private const int FIVES = 4;
        private const int SIXES = 5;
        private const int THREE_OF_A_KIND = 6;
        private const int FOUR_OF_A_KIND = 7;
        private const int FULL_HOUSE = 8;
        private const int SMALL_STRAIGHT = 9;
        private const int LARGE_STRAIGHT = 10;
        private const int CHANCE = 11;
        private const int YAHTZEE = 12;

        private int rollCount = 0;
        private int uScoreCardCount = 0;

        // you'll need an instance variable for the user's scorecard - an array of 13 ints
        private int[] userScorecardArray = new int[13];
        // as well as an instance variable for 0 to 5 dice as the user rolls - array or list<int>?
        private List<int> userRollsList = new List<int>(5);
        // as well as an instance variable for 0 to 5 dice that the user wants to keep - array or list<int>? 
        private List<int> userKeepList = new List<int>(5);
        // this is the list of methods that I used

        // START WITH THESE 2
        // This method rolls numDie and puts the results in the list
        public void Roll(int numDie, List<int> dice)
        {
            // Random number generator
            Random rand = new Random();

            // for each die starting at 0 and less than 5, incremented
            //      generate random number between 0 and 5
            //      add random number to list
            for (int i = 0; i < 5; i++)
            {
                numDie = rand.Next(6);
                dice.Add(numDie);
            }
        }

        // This method moves all of the rolled dice to the keep dice before scoring.  All of the dice that
        // are being scored have to be in the same list 
        public void MoveRollDiceToKeep(List<int> roll, List<int> keep)
        {
            // List to hold keep die
            keep = new List<int>();

            foreach (int r in roll)
            {
                keep.Add(r);
            }
        }

        #region Scoring Methods
        /* This method returns the number of times the parameter value occurs in the list of dice.
         * Calling it with 5 and the following dice 2, 3, 5, 5, 6 returns 2.
         */
        private int Count(int value, List<int> dice)
        {
            // Variable and initialization for count
            int count = 0;

            foreach (int die in dice)
            {
                if (die == value)
                    count++;
            }
            return count;
        }

        /* This method counts how many 1s, 2s, 3s ... 6s there are in a list of ints that represent a set of dice
         * It takes a list of ints as its parameter.  It should create an array of 6 integers to store the counts.
         * It should then call Count with a value of 1 and store the result in the first element of the array.
         * It should then repeat the process of calling Count with 2 - 6.
         * It returns the array of counts.
         * All of the rest of the scoring methods can be "easily" calculated using the array of counts.
         */
        private int[] GetCounts(List<int> dice)
        {
            // Array to store counts
            int[] counts = new int[6];

            // Count the amount of each number rolled and store result in a slot in array.
            counts[0] = Count(1, dice);
            counts[1] = Count(2, dice);
            counts[2] = Count(3, dice);
            counts[3] = Count(4, dice);
            counts[4] = Count(5, dice);
            counts[5] = Count(6, dice);
            return counts;
        }

        /* Each of these methods takes the array of counts as a parameter and returns the score for a dice value.
         */
        private int ScoreOnes(int[] counts)
        {
            // Variable for number of 1s (slot 0 in array counts)
            int one = counts[0];

            // if one is greater than zero
            //     return ones
            // else return 0
            if (one > 0)
                return one;
            else
                return 0;
        }

        private int ScoreTwos(int[] counts)
        {
            // Variable for number of 2s (slot 1 in array counts)
            int two = counts[1];

            // if two is greater than 0
            //    two = two * 2
            //    return two
            // else return 0
            if (two > 0)
            {
                two = two * 2;
                return two;
            }
            else
                return 0;
        }

        private int ScoreThrees(int[] counts)
        {
            // Variable for number of 3s (slot 2 in array counts)
            int three = counts[2];

            // if three is greater than 0
            //    three = three * 3
            //    return three
            // else return 0

            if (three > 0)
            {
                three = three * 3;
                return three;
            }
            else
                return 0;
        }

        private int ScoreFours(int[] counts)
        {
            // Variable for number of 4s (slot 3 in array counts)
            int four = counts[3];

            if (four > 0)
            {
                four = four * 4;
                return four;
            }
            else
                return 0;
        }

        private int ScoreFives(int[] counts)
        {
            // Variable for number of 5s (slot 4 in array counts)
            int five = counts[4];

            if (five > 0)
            {
                five = five * 5;
                return five;
            }
            else
                return 0;
        }

        private int ScoreSixes(int[] counts)
        {
            // Variable for number of 6s (slot 5 in array counts)
            int six = counts[5];

            if (six > 0)
            {
                six = six * 6;
                return six;
            }
            else
                return 0;
        }

        /* This method can be used to determine if you have 3 of a kind (or 4? or  5?).  The output parameter
         * whichValue tells you which dice value is 3 of a kind.
         */ 
        private bool HasCount(int howMany, int[] counts, out int whichValue)
        {
            int index = ONES;
            foreach (int count in counts)
            {
                if (howMany == count)
                {
                    whichValue = index;
                    return true;
                }
            }
            whichValue = NONE;
            return false;
        }

        /* This method returns the sum of the dice represented in the counts array.
         * The sum is the # of 1s * 1 + the # of 2s * 2 + the number of 3s * 3 etc
         */ 
        private int Sum(int[] counts)
        {
            int sum = ScoreOnes(counts) + ScoreTwos(counts) + ScoreThrees(counts)
                        + ScoreFours(counts) + ScoreFives(counts) + ScoreSixes(counts);
            return sum;
        }

        /* This method calls HasCount(3...) and if there are 3 of a kind calls Sum to calculate the score.
         */ 
        private int ScoreThreeOfAKind(int[] counts)
        {
            int score;
            HasCount(3, counts, out score);
            return score;
        }

        private int ScoreFourOfAKind(int[] counts)
        {
            int score;
            HasCount(4, counts, out score);
            return score;
        }

        private int ScoreYahtzee(int[] counts)
        {
            int score;
            HasCount(5, counts, out score);
            return score;
        }

        /* This method calls HasCount(2 and HasCount(3 to determine if there's a full house.  It calls sum to 
         * calculate the score.
         */ 
        private int ScoreFullHouse(int[] counts)
        {
            int scores2, scores3;
            if (HasCount(2, counts, out scores2) && HasCount(3, counts, out scores3))
            {
               int score = Sum(counts);
                return score;
            }
            else
                return 0;
        }

        private int ScoreSmallStraight(int[] counts)
        {
            // Bool to hold true or false value for small straight check,
            //     initialized as true
            bool isSmStraight = true;

            // for the first 3 slots of the counts array, incremented
            //    for the 4 slots following the index of the first index
            //         if the slot is less than 1
            //              small straight check results in false value, break
            //    if small straight remains true, break
            for (int i = 0; i < 3; i++)
            {
                for (int j = i; j < i + 4; j++)
                {
                    if (counts[j] < 1)
                    {
                        isSmStraight = false;
                        break;
                    }
                }
                if (isSmStraight == true)
                    break;
            }
            // if is small straight is true
            //     return 30 points
            if (isSmStraight == true)
            {
                int score = 30;
                return score;
            }
            else
                return 0;
        }

        private int ScoreLargeStraight(int[] counts)
        {
            // Bool to hold true or false value for large straight check,
            //    initialized as true.
            bool isLargeStraight = true;

            // for each slot of the array beginning at 0 and less than 6, incremented
            //     if the slot is not equal to 1
            //        large straight check results in false value, break
            for (int i = 0; i < 6; i++)
            {
                if (counts[i] != 1)
                {
                    isLargeStraight = false;
                    break;
                }
            }
            // if large straight is true
            //    return 40 points
            if (isLargeStraight == true)
            {
                int score = 40;
                return score;
            }
            else
                return 0;
        }

        private int ScoreChance(int[] counts)
        {
            int chanceScore = Sum(counts);
            return chanceScore;
        }

        /* This method makes it "easy" to call the "right" scoring method when you click on an element
         * in the user score card on the UI.
         */ 
        private int Score(int whichElement, List<int> dice)
        {
            int[] counts = GetCounts(dice);
            switch (whichElement)
            {
                case ONES:
                    return ScoreOnes(counts);
                case TWOS:
                    return ScoreTwos(counts);
                case THREES:
                    return ScoreThrees(counts);
                case FOURS:
                    return ScoreFours(counts);
                case FIVES:
                    return ScoreFives(counts);
                case SIXES:
                    return ScoreSixes(counts);
                case THREE_OF_A_KIND:
                    return ScoreThreeOfAKind(counts);
                case FOUR_OF_A_KIND:
                    return ScoreFourOfAKind(counts);
                case FULL_HOUSE:
                    return ScoreFullHouse(counts);
                case SMALL_STRAIGHT:
                    return ScoreSmallStraight(counts);
                case LARGE_STRAIGHT:
                    return ScoreLargeStraight(counts);
                case CHANCE:
                    return ScoreChance(counts);
                case YAHTZEE:
                    return ScoreYahtzee(counts);
                default:
                    return 0;
            }
        }
        #endregion

        // set each value to some negative number because 
        // a 0 or a positive number could be an actual score
        private void ResetScoreCard(int[] scoreCard, int scoreCardCount)
        {
            // for all the slots in the score card array, incremented
            //     set slot to 0
            for (int i = 0; i <6; i++)
            {
                scoreCard[i] = 0;
            }
            // set score card count to 0
            scoreCardCount = 0;
        }

        // this set has to do with user's scorecard UI
        private void ResetUserUIScoreCard()
        {
            uScoreCardCount = 0;
        }

        // this method adds the subtotals as well as the bonus points when the user is done playing
        public void UpdateUserUIScoreCard()
        {

        }

        /* When I move a die from roll to keep, I put a -1 in the spot that the die used to be in.
         * This method gets rid of all of those -1s in the list.
         */
        private void CollapseDice(List<int> dice)
        {
            int numDice = dice.Count;
            for (int count = 0, i = 0; count < numDice; count++)
            {
                if (dice[i] == -1)
                    dice.RemoveAt(i);
                else
                    i++;
            }
        }

        /* When I move a die from roll to keep, I need to know which pb I can use.  It's the first spot with a -1 in it
         */
        public int GetFirstAvailablePB(List<int> dice)
        {
            return dice.IndexOf(-1);
        }

        #region UI Dice Methods
        /* These are all UI methods */
        private PictureBox GetKeepDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["keep" + i];
            return die;
        }

        public void HideKeepDie(int i)
        {
            GetKeepDie(i).Visible = false;
        }
        public void HideAllKeepDice()
        {
            for (int i = 0; i < 5; i++)
                HideKeepDie(i);
        }

        public void ShowKeepDie(int i)
        {
            PictureBox die = GetKeepDie(i);
            //die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + keep[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllKeepDie()
        {
            for (int i = 0; i < 5; i++)
                ShowKeepDie(i);
        }

        private PictureBox GetComputerKeepDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["computerKeep" + i];
            return die;
        }

        public void HideComputerKeepDie(int i)
        {
            GetComputerKeepDie(i).Visible = false;
        }

        public void HideAllComputerKeepDice()
        {
            for (int i = 0; i < 5; i++)
                HideComputerKeepDie(i);
        }

        public void ShowComputerKeepDie(int i)
        {
            PictureBox die = GetComputerKeepDie(i);
            //die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + keep[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllComputerKeepDie()
        {
            for (int i = 0; i < 5; i++)
                ShowComputerKeepDie(i);
        }

        private PictureBox GetRollDie(int i)
        {
            PictureBox die = (PictureBox)this.Controls["roll" + i];
            return die;
        }

        public void HideRollDie(int i)
        {
            GetRollDie(i).Visible = false;
        }

        public void HideAllRollDice()
        {
            for (int i = 0; i < 5; i++)
                HideRollDie(i);
        }

        public void ShowRollDie(int i)
        {
            PictureBox die = GetRollDie(i);
            //die.Image = Image.FromFile(System.Environment.CurrentDirectory + "\\..\\..\\Dice\\die" + roll[i] + ".png");
            die.Visible = true;
        }

        public void ShowAllRollDie()
        {
            for (int i = 0; i < 5; i++)
                ShowRollDie(i);
        }
        #endregion

        #region Event Handlers
        private void Form1_Load(object sender, EventArgs e)
        {
            /* reset the user's scorecard
            * Hide the roll dice
            * Hide the keep dice
            * Hide the computer's dice
            */
            ResetScoreCard(userScorecardArray, uScoreCardCount);
            HideAllRollDice();
            HideAllKeepDice();
            HideAllComputerKeepDice();
        }

        private void rollButton_Click(object sender, EventArgs e)
        {
            // DON'T WORRY ABOUT ROLLING MULTIPLE TIMES UNTIL YOU CAN SCORE ONE ROLL
            // hide all of the keep picture boxes
            // any of the die that were moved back and forth from roll to keep by the user
            // are "collapsed" in the keep data structure
            // show the keep dice again

            // START HERE
            // clear the roll data structure

            // hide all of thhe roll picture boxes
            HideAllRollDice();

            // roll the right number of dice

            // show the roll picture boxes

            // increment the number of rolls
            rollCount++;
            // disable the button if you've rolled 3 times
            if (rollCount > 2)
            {
                rollButton.Enabled = false;
            }
        }

        private void userScorecard_DoubleClick(object sender, EventArgs e)
        {
            // move any rolled die into the keep dice
            CollapseDice(userRollsList);
            // hide picture boxes for both roll and keep
            HideAllRollDice();
            HideAllKeepDice();

            // determine which element in the score card was clicked

            // score that element
            // put the score in the scorecard and the UI
            // disable this element in the score card

            // clear the keep dice
            // reset the roll count
            // increment the number of elements in the score card that are full
            // enable/disable buttons

            // when it's the end of the game
            // update the sum(s) and bonus parts of the score card
            // enable/disable buttons
            // display a message box?
        }

        private void roll_DoubleClick(object sender, EventArgs e)
        {
            // figure out which die you clicked on

            // figure out where in the set of keep picture boxes there's a "space"
            // move the roll die value from this die to the keep data structure in the "right place"
            // sometimes that will be at the end but if the user is moving dice back and forth
            // it may be in the middle somewhere

            // clear the die in the roll data structure
            // hide the picture box
        }

        private void keep_DoubleClick(object sender, EventArgs e)
        {
            // figure out which die you clicked on

            // figure out where in the set of roll picture boxes there's a "space"
            // move the roll die value from this die to the roll data structure in the "right place"
            // sometimes that will be at the end but if the user is moving dice back and forth
            // it may be in the middle somewhere

            // clear the die in the keep data structure
            // hide the picture box
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}

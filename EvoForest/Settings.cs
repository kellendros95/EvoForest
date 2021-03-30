using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EvoForest
{
    static class Settings
    {
        static Color[] _geneColors;

        // Общие параметры мира
        public const float MutationChance = 0.01f;
        //public const int MaxAge = 2000;
        //public const int YearLength = 100;
        public const int BottomY = 10;
        public const int MaxX = 200;
        public const float Light = 0.5f;
        public const float BaseTreeLoss = 0.05f;
        public const float InitTreeEnergy = 10.0f;

        // Параметры генов и ДНК
        public const int DnaLen = 27;
        public const int GrowVariants = 2;
        public const int NextGeneVariants = 2;
        public const int GeneSize = 2 + GrowVariants + NextGeneVariants;

        // Параметры листьев
        public const float LeafLoss = 0.5f;
        public const float LeafCost = 1.0f;
        public const float LeafMass = 1.0f;
        public const float MinLeafRadius = 0.1f;
        public const float MaxLeafRadius = 0.3f;

        // Параметры веток
        public const float BranchLoss = 0.01f;
        public const float BranchCost = 1.0f;
        public const float BranchMass = 2.0f;
        public const float MinBranchLength = 0.2f;
        public const float MaxBranchLength = 0.5f;
        public const float ThicknessScale = 0.05f;
        public const float MaxThickness = 10.0f;
        public const float MaxMomentum = 100.0f;
        public const float MaxMass = 100.0f;

        // Параметры семян
        public const float SeedEnergyEfficiency = 0.5f;
        public const float MaxSeedEnergy = 40.0f;
        public const float MaxSeedScatter = 2.0f;
        public const float SeedScatterCost = 5.0f;
        static public Color GeneColor(int i)
            => _geneColors[i];
        static public void InitColors()
        {
            _geneColors = new Color[27];
            for (int i = 0; i < 27; i++)
                _geneColors[i] = new Color(
                    (byte)(50 + 90 * (i % 3)),
                    (byte)(50 + 90 * ((i / 3) % 3)),
                    (byte)(50 + 90 * ((i / 9) % 3)));
        }
    }
}

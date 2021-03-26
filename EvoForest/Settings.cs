using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvoForest
{
    static class Settings
    {
        // Общие параметры мира
        public const float MutationChance = 0.2f;
        public const int MaxAge = 2000;
        public const int YearLength = 100;
        public const int BottomY = 14;
        public const int MaxX = 500;
        public const float Light = 0.1f;

        // Параметры генов и ДНК
        public const int DnaLen = 20;
        public const int GrowVariants = 1;
        public const int NextGeneVariants = 2;
        public const int GeneSize = 2 + GrowVariants + NextGeneVariants;

        // Параметры листьев
        public const float LeafLoss = 0.01f;
        public const float LeafCost = 0.02f;
        public const float LeafMass = 1.0f;
        public const float MinLeafRadius = 0.1f;
        public const float MaxLeafRadius = 0.3f;

        // Параметры веток
        public const float BranchLoss = 0.0f;
        public const float BranchCost = 2.0f;
        public const float BranchMass = 5.0f;
        public const float MinBranchLength = 0.2f;
        public const float MaxBranchLength = 0.5f;

        // Параметры семян
        public const float SeedEnergyEfficiency = 0.5f;
        public const float MaxSeedEnergy = 40.0f;
        public const float MaxSeedScatter = 2.0f;
        public const float SeedScatterCost = 5.0f;
        
    }
}

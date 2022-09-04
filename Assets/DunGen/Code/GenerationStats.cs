using System.Diagnostics;

namespace DunGen
{
	public sealed class GenerationStats
	{
		public int MainPathRoomCount { get; private set; }
		public int BranchPathRoomCount { get; private set; }
		public int TotalRoomCount { get; private set; }
		public int MaxBranchDepth { get; private set; }
		public int TotalRetries { get; private set; }

		public int PrunedBranchTileCount { get; internal set; }

		public float PreProcessTime { get; private set; }
		public float MainPathGenerationTime { get; private set; }
		public float BranchPathGenerationTime { get; private set; }
		public float PostProcessTime { get; private set; }
		public float TotalTime { get; private set; }

		private Stopwatch stopwatch = new Stopwatch();
		private GenerationStatus generationStatus;


		internal void Clear()
		{
			MainPathRoomCount = 0;
			BranchPathRoomCount = 0;
			TotalRoomCount = 0;
			MaxBranchDepth = 0;
			TotalRetries = 0;
			PrunedBranchTileCount = 0;

			PreProcessTime = 0f;
			MainPathGenerationTime = 0f;
			BranchPathGenerationTime = 0f;
			PostProcessTime = 0f;
			TotalTime = 0f;
		}

		internal void IncrementRetryCount()
		{
			TotalRetries++;
		}

		internal void SetRoomStatistics(int mainPathRoomCount, int branchPathRoomCount, int maxBranchDepth)
		{
			MainPathRoomCount = mainPathRoomCount;
			BranchPathRoomCount = branchPathRoomCount;
			MaxBranchDepth = maxBranchDepth;
			TotalRoomCount = MainPathRoomCount + BranchPathRoomCount;
		}

		internal void BeginTime(GenerationStatus status)
		{
			if (stopwatch.IsRunning)
				EndTime();

			generationStatus = status;
			stopwatch.Reset();
			stopwatch.Start();
		}

		internal void EndTime()
		{
			stopwatch.Stop();
			float elapsedTime = (float)stopwatch.Elapsed.TotalMilliseconds;

			switch (generationStatus)
			{
				case GenerationStatus.PreProcessing:
					PreProcessTime += elapsedTime;
					break;
				case GenerationStatus.MainPath:
					MainPathGenerationTime += elapsedTime;
					break;
				case GenerationStatus.Branching:
					BranchPathGenerationTime += elapsedTime;
					break;
				case GenerationStatus.PostProcessing:
					PostProcessTime += elapsedTime;
					break;
			}

			TotalTime = PreProcessTime + MainPathGenerationTime + BranchPathGenerationTime + PostProcessTime;
		}

		public GenerationStats Clone()
		{
			GenerationStats newStats = new GenerationStats();

			newStats.MainPathRoomCount = MainPathRoomCount;
			newStats.BranchPathRoomCount = BranchPathRoomCount;
			newStats.TotalRoomCount = TotalRoomCount;
			newStats.MaxBranchDepth = MaxBranchDepth;
			newStats.TotalRetries = TotalRetries;
			newStats.PrunedBranchTileCount = PrunedBranchTileCount;

			newStats.PreProcessTime = PreProcessTime;
			newStats.MainPathGenerationTime = MainPathGenerationTime;
			newStats.BranchPathGenerationTime = BranchPathGenerationTime;
			newStats.PostProcessTime = PostProcessTime;
			newStats.TotalTime = TotalTime;

			return newStats;
		}
	}
}

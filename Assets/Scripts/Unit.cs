using System;
using UnityEngine;
using System.Collections.Generic; 
//using System.;

namespace ToyTown
{
		public enum UnitJob {
			NOTHING,
			FARMER,
			LUMBERJACK,
			MINER,
			BUILDER,
		};

		public enum UnitAction {
			WANDERING,
			WORKING,
			LEARNING,
		};

		public static class ActionBuilder
		{
			public static Func<Unit, float, int?> ScoreAddByDay(double saturationByDay = 0, double energyByDay = 0, double happynessByDay = 0)
			{
				return (Unit unit, float delta) =>
				{
								unit.saturationScore = saturationByDay;
								unit.happynessScore = happynessByDay;
								unit.energyScore = energyByDay;
					return 0;
				};
			}
		};


		public class Unit : MonoBehaviour
		{
			public static Dictionary<UnitAction, Func<Unit, float, int?>> ActionDoingDictionnary = new()
			{
				{UnitAction.WANDERING, ActionBuilder.ScoreAddByDay(saturationByDay: -.2)},
				{UnitAction.WORKING, ActionBuilder.ScoreAddByDay(saturationByDay: -.2)},
				{UnitAction.LEARNING, ActionBuilder.ScoreAddByDay(saturationByDay: -.2)},
			};

			public double saturationScore = 1;
			public double energyScore = 1;
			public double happynessScore = .5;
			public UnitAction actualAction = UnitAction.WANDERING;
			public UnitJob actualJob = UnitJob.NOTHING;
				
			// Start is called once before the first execution of Update after the MonoBehaviour is created
			void Start()
			{
				
			}

			// Update is called once per frame
			void Update()
			{
				ActionDoingDictionnary[this.actualAction](this, Time.deltaTime);
			}
		}
}

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
			EATING,
			SLEEPING,
			WALKING,
		};

		public enum ActionReturn {
			CONTINUE,
			DONE,
		};

		public static class ActionFunctionBuilder
		{
			public static Func<Unit, float, ActionReturn> ScoreAddByDay(double saturationByDay = 0, double energyByDay = 0, double happynessByDay = 0)
			{
				return (Unit unit, float delta) =>
				{
					double factor = delta / Settings.DayLengthInSecond;
					unit.saturationScore += saturationByDay * factor;
					unit.happynessScore += happynessByDay * factor;
					unit.energyScore += energyByDay * factor;
					return ActionReturn.CONTINUE;
				};
			}
			
			public static Func<Unit, float, ActionReturn> ScoreAddByAction(double saturationByAction = 0, double energyByAction = 0, double happynessByAction = 0)
			{
				return (Unit unit, float delta) =>
				{
					if (unit.actionSystemDaysRemain <= 0) return ActionReturn.DONE;
					double factor = delta * unit.actionSystemDaysAmount / Settings.DayLengthInSecond;
					unit.saturationScore += saturationByAction * factor;
					unit.happynessScore += happynessByAction * factor;
					unit.energyScore += energyByAction * factor;
					unit.actionSystemDaysRemain -= delta / Settings.DayLengthInSecond;
					return ActionReturn.CONTINUE;
				};
			}
		};
	
		public static class Action
		{
			public static Dictionary<UnitAction, Func<Unit, float, ActionReturn>> Dictionnary = new()
			{
				// action order
				{UnitAction.WANDERING, ActionFunctionBuilder.ScoreAddByDay(saturationByDay: -.2, energyByDay: -.2, happynessByDay: -.1)},
				{UnitAction.WORKING, ActionFunctionBuilder.ScoreAddByDay(saturationByDay: -.5, energyByDay: -.5, happynessByDay: .1)},
				{UnitAction.LEARNING, ActionFunctionBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0)},
				// action system
				{UnitAction.EATING, ActionFunctionBuilder.ScoreAddByAction(saturationByAction: .5)},
				{UnitAction.SLEEPING, ActionFunctionBuilder.ScoreAddByAction(energyByAction: 1)},
				// between action
				{UnitAction.WALKING, ActionFunctionBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0)},
			};
		}



		public class Unit : MonoBehaviour
		{

			public double saturationScore = 1;
			public double energyScore = 1;
			public double happynessScore = .5;
			public UnitAction actionOrder = UnitAction.WANDERING;
			public UnitAction? actionSystem = null;
			public double actionSystemDaysAmount = .0;
			public double actionSystemDaysRemain = .0;
			public UnitJob actualJob = UnitJob.NOTHING;
			public UnitJob? learningJob = null;

			// Switching actual action
			void SwtichAction()
			{
				
			}

			// Start is called once before the first execution of Update after the MonoBehaviour is created
			void Start()
			{
				
			}

			// Update is called once per frame
			void Update()
			{
				Action.Dictionnary[this.actionOrder](this, Time.deltaTime);
			}
		}
}

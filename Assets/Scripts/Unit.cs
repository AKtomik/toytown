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

	public enum ActionUpdateReturn {
		CONTINUE,
		DONE,
	};

	public static class ActionStartBuilder
	{	
		public static void Default(Unit unit)
		{
			unit.actionSystemDaysAmount = 0;
			unit.actionSystemDaysRemain = 0;
		}

		public static Action<Unit> StartTimer(double timerDayAmount)
		{
			return (Unit unit) =>
			{
				unit.actionSystemDaysAmount = timerDayAmount;
				unit.actionSystemDaysRemain = timerDayAmount;
			};
		}
	};

	public static class ActionUpdateBuilder
	{
		public static ActionUpdateReturn Default(Unit unit, float delta)
		{
			return ActionUpdateReturn.CONTINUE;
		}

		public static Func<Unit, float, ActionUpdateReturn> ScoreAddByDay(double saturationByDay = 0, double energyByDay = 0, double happynessByDay = 0)
		{
			return (Unit unit, float delta) =>
			{
				double factor = delta / Settings.DayLengthInSecond;
				unit.saturationScore += saturationByDay * factor;
				unit.happynessScore += happynessByDay * factor;
				unit.energyScore += energyByDay * factor;
				return ActionUpdateReturn.CONTINUE;
			};
		}
		
		public static Func<Unit, float, ActionUpdateReturn> ScoreAddByAction(double saturationByAction = 0, double energyByAction = 0, double happynessByAction = 0)
		{
			return (Unit unit, float delta) =>
			{
				if (unit.actionSystemDaysRemain <= 0) return ActionUpdateReturn.DONE;
				double factor = delta * unit.actionSystemDaysAmount / Settings.DayLengthInSecond;
				unit.saturationScore += saturationByAction * factor;
				unit.happynessScore += happynessByAction * factor;
				unit.energyScore += energyByAction * factor;
				unit.actionSystemDaysRemain -= delta / Settings.DayLengthInSecond;
				return ActionUpdateReturn.CONTINUE;
			};
		}
	};

	public class Action
	{
		public Action<Unit> Start;
		public Func<Unit, float, ActionUpdateReturn> Update;
		
		public Action(Action<Unit> start = null, Func<Unit, float, ActionUpdateReturn> update = null)
		{
			Start = start ?? ActionStartBuilder.Default;
			Update = update ?? ActionUpdateBuilder.Default;
		}
		
		public static Dictionary<UnitAction, Action> Dictionnary = new()
		{
			// action order
			{UnitAction.WANDERING, new Action(update: ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.2, energyByDay: -.2, happynessByDay: -.1))},
			{UnitAction.WORKING, new Action(update: ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.5, energyByDay: -.5, happynessByDay: .1))},
			{UnitAction.LEARNING, new Action(update: ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0))},
			// action system
			{UnitAction.EATING, new Action(update: ActionUpdateBuilder.ScoreAddByAction(saturationByAction: .5))},
			{UnitAction.SLEEPING, new Action(update: ActionUpdateBuilder.ScoreAddByAction(energyByAction: 1))},
			// between action
			{UnitAction.WALKING, new Action(update: ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0))},
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
			Action.Dictionnary[this.actionOrder].Update(this, Time.deltaTime);
		}
	}
}

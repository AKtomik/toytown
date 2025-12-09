using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//using System.;

namespace ToyTown
{
	using ActionStartFunction = Action<Unit>;
	using ActionUpdateFunction = Func<Unit, float, ActionUpdateReturn>;

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

	public enum UnitActionPlayer {
		WANDERING = UnitAction.WANDERING,
		WORKING = UnitAction.WORKING,
		LEARNING = UnitAction.LEARNING,
	};

	public enum UnitActionSystem {
		EATING = UnitAction.EATING,
		SLEEPING = UnitAction.SLEEPING,
		WALKING = UnitAction.WALKING,
	};

	public enum ActionUpdateReturn {
		CONTINUE,
		DONE,
	};

	public class Action
	{
		public ActionStartFunction Start;
		public ActionUpdateFunction Update;
		
		public Action(ActionStartFunction start = null, ActionUpdateFunction update = null)
		{
			Start = start ?? Unit.ActionStartBuilder.Default;
			Update = update ?? Unit.ActionUpdateBuilder.Default;
		}
		
		public static Dictionary<UnitAction, Action> Dictionnary = new()
		{
			// action order
			{UnitAction.WANDERING, new Action(
				update: Unit.ActionUpdateBuilder.Merge(Unit.ActionUpdateBuilder.Wander, Unit.ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.2, energyByDay: -.2, happynessByDay: -.1))
				)},
			{UnitAction.WORKING, new Action(
				start: Unit.ActionStartBuilder.Merge(Unit.ActionStartBuilder.Job, Unit.ActionStartBuilder.GoingToWork),
				update: Unit.ActionUpdateBuilder.Merge(Unit.ActionUpdateBuilder.Job, Unit.ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.5, energyByDay: -.5, happynessByDay: .1))
				)},
			{UnitAction.LEARNING, new Action(
				start: Unit.ActionStartBuilder.Merge(Unit.ActionStartBuilder.Learn, Unit.ActionStartBuilder.GoingToPlace(Place.SCHOOL)),
				update: Unit.ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0)
				)},
			// action system
			{UnitAction.EATING, new Action(
				start: Unit.ActionStartBuilder.Merge(Unit.ActionStartBuilder.StartTimer(timerDayAmount: .05), Unit.ActionStartBuilder.GoingToPlace(Place.CANTINE)),
				update: Unit.ActionUpdateBuilder.ScoreAddByAction(saturationByAction: 1)
				)},
			{UnitAction.SLEEPING, new Action(
				start: Unit.ActionStartBuilder.Merge(Unit.ActionStartBuilder.StartTimer(timerDayAmount: .5), Unit.ActionStartBuilder.GoingToPlace(Place.HOUSE)),
				update: Unit.ActionUpdateBuilder.ScoreAddByAction(energyByAction: 1)
				)},
			// between action
			{UnitAction.WALKING, new Action(
				update: Unit.ActionUpdateBuilder.ScoreAddByDay(saturationByDay: -.3, energyByDay: -.5, happynessByDay: 0)
				)},
		};
	}

	[RequireComponent(typeof(Rigidbody))]
	public class Unit : MonoBehaviour
	{

		public static class ActionStartBuilder
		{	
			// raw functions
			public static void Default(Unit unit)
			{
				unit.actionSystemDaysAmount = 0;
				unit.actionSystemDaysRemain = 0;
			}
			
			public static void Learn(Unit unit)
			{
				unit.learningRemainDay = Settings.UnitLearningTimeDay;
			}

			public static void Job(Unit unit)
			{
				JobsDictionnary[unit.GetActualJob()](unit);
			}
			
			public static void GoingToWork(Unit unit)
			{
				unit.walkingObjective = GameManager.Instance.GetNearestPlace((Place)unit.GetActualJob(), unit.transform.position);
			}

			// jobs functions
			public static Dictionary<UnitJob, ActionStartFunction> JobsDictionnary = new()
			{
				{ UnitJob.FARMER, unit => {
					
				} },
				{ UnitJob.LUMBERJACK, unit => {
					
				} },
				{ UnitJob.MINER, unit => {
					
				} },
				{ UnitJob.BUILDER, unit => {
					
				} }
			};

			// functions builder
			public static ActionStartFunction Merge(ActionStartFunction Action1, ActionStartFunction Action2)
			{
				return (unit) =>
				{
					Action1(unit);
					Action2(unit);
				};
			}

			public static ActionStartFunction StartTimer(double timerDayAmount)
			{
				return unit =>
				{
					unit.actionSystemDaysAmount = timerDayAmount;
					unit.actionSystemDaysRemain = timerDayAmount;
				};
			}

			public static ActionStartFunction GoingToPlace(Place place)
			{
				return unit =>
				{
					unit.walkingObjective = GameManager.Instance.GetNearestPlace(place, unit.transform.position);
				};
			}
		};

		public static class ActionUpdateBuilder
		{
			// raw functions
			public static ActionUpdateReturn Default(Unit unit, float delta)
			{
				return ActionUpdateReturn.CONTINUE;
			}

			public static ActionUpdateReturn Wander(Unit unit, float delta)
			{
				// TODO : implement
				return ActionUpdateReturn.CONTINUE;
			}
			
			public static ActionUpdateReturn Learn(Unit unit, float delta)
			{
				unit.learningRemainDay -= delta / Settings.DayLengthInSecond;
				if (unit.learningRemainDay <= 0)
				{
					unit.SwtichJob((UnitJob)unit.learningJob);
					unit.learningJob = null;
					unit.learningRemainDay = 0;
					return ActionUpdateReturn.DONE;
				}
				return ActionUpdateReturn.CONTINUE;
			}
			
			public static ActionUpdateReturn Job(Unit unit, float delta)
			{
				return JobsDictionnary[unit.GetActualJob()](unit, delta);
			}

			// jobs functions
			public static Dictionary<UnitJob, ActionUpdateFunction> JobsDictionnary = new()
			{
				{ UnitJob.FARMER, (unit, delta) => {
					return ActionUpdateReturn.CONTINUE;
				} },
				{ UnitJob.LUMBERJACK, (unit, delta) => {
					return ActionUpdateReturn.CONTINUE;
				} },
				{ UnitJob.MINER, (unit, delta) => {
					return ActionUpdateReturn.CONTINUE;
				} },
				{ UnitJob.BUILDER, (unit, delta) => {
					return ActionUpdateReturn.CONTINUE;
				} }
			};

			// functions builder
			public static ActionUpdateFunction Merge(ActionUpdateFunction Action1, ActionUpdateFunction Action2)
			{
				return (unit, delta) =>
				{
					ActionUpdateReturn r1 = Action1(unit, delta);
					ActionUpdateReturn r2 = Action2(unit, delta);
					return (r1 > r2) ? r1 : r2;
				};
			}

			public static ActionUpdateFunction ScoreAddByDay(double saturationByDay = 0, double energyByDay = 0, double happynessByDay = 0)
			{
				return (unit, delta) =>
				{
					double factor = delta / Settings.DayLengthInSecond;
					unit.saturationScore += saturationByDay * factor;
					unit.happynessScore += happynessByDay * factor;
					unit.energyScore += energyByDay * factor;
					return ActionUpdateReturn.CONTINUE;
				};
			}
			
			public static ActionUpdateFunction ScoreAddByAction(double saturationByAction = 0, double energyByAction = 0, double happynessByAction = 0)
			{
				return (unit, delta) =>
				{
					if (unit.actionSystemDaysRemain <= 0) return ActionUpdateReturn.DONE;
					unit.actionSystemDaysRemain -= delta / Settings.DayLengthInSecond;
					double factor = delta  / Settings.DayLengthInSecond / unit.actionSystemDaysAmount;
					unit.saturationScore += saturationByAction * factor;
					unit.happynessScore += happynessByAction * factor;
					unit.energyScore += energyByAction * factor;
					return ActionUpdateReturn.CONTINUE;
				};
			}
		};

		
		private Rigidbody rb;

		public double saturationScore = 1;
		public double energyScore = 1;
		public double happynessScore = .5;
		
		private UnitActionPlayer actionPlayer = UnitActionPlayer.WANDERING;
		private UnitActionSystem? actionSystem = null;
		private double actionSystemDaysAmount = .0;
		private double actionSystemDaysRemain = .0;
		private Vector3? walkingObjective = null;

		private UnitJob actualJob = UnitJob.NOTHING;
		
		private UnitJob? learningJob = null;
		private double learningRemainDay;
		public double learningProgress
		{
			get
			{
				return 1 - (learningRemainDay / Settings.UnitLearningTimeDay);
			}
		}

		// Switching actual action

		public void SwtichSystemAction(UnitActionSystem action)
		{
			Action.Dictionnary[(UnitAction)action].Start(this);
			actionSystem = action;
		}
		
		public void EndSystemAction()
		{
			actionSystem = null;
		}
		
		public void SwtichPlayerAction(UnitActionPlayer action)
		{
			EndSystemAction();//!
			Action.Dictionnary[(UnitAction)action].Start(this);
			actionPlayer = action;
		}
		
		public void StartLearning(UnitJob job)
		{
			learningJob = job;
			SwtichPlayerAction(UnitActionPlayer.LEARNING);
		}
		
		public void SwtichJob(UnitJob job)
		{
			actualJob = job;
		}

		public UnitJob GetActualJob()
		{
			return actualJob;
		}

		public UnitAction GetActualAction()
		{
			if (actionSystem != null)
				return (UnitAction)actionSystem;
			else
				return (UnitAction)actionPlayer;
		}

		public bool IsHungry()
		{
			return saturationScore < Settings.UnitLetdownPoint;
		}
		
		public bool IsTired()
		{
			return energyScore < Settings.UnitLetdownPoint;
		}

		public bool IsWalking()
		{
			return walkingObjective != null && Vector3.Distance((Vector3)walkingObjective, transform.position) > Settings.WalkingNearObjectiveDistance;
		}
		
		public double speed
		{
			get {
				return Settings.UnitBaseSpeed * (IsHungry() ? .5 : 1) * (IsTired() ? .5 : 1);
			}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			rb = GetComponent<Rigidbody>();
			if (!Action.Dictionnary.Keys.Contains((UnitAction)this.actionPlayer))
			{
				Debug.LogError($"actionPlayer is not a correct UnitAction! Please choose a value for {this}.actionPlayer. (this.actionPlayer = {this.actionPlayer})");
				this.actionPlayer = UnitActionPlayer.WANDERING;
			}
			// ! test
			SwtichJob(UnitJob.MINER);
			SwtichPlayerAction(UnitActionPlayer.WORKING);
		}
		// Update is called once per frame
		void Update()
		{
			// if walking
			if (IsWalking())
			{
				rb.MovePosition(Vector3.MoveTowards(transform.position, (Vector3)walkingObjective, (float)(speed * Settings.WalkingSpeed * Time.deltaTime)));
				Action.Dictionnary[UnitAction.WALKING].Update(this, Time.deltaTime * (float)speed);
			}
			else
			{
				ActionUpdateReturn actionFeedback = Action.Dictionnary[GetActualAction()].Update(this, Time.deltaTime * (float)speed);

				// if action done
				if (actionFeedback == ActionUpdateReturn.DONE)
				{
					if (actionSystem != null)
					{
						actionSystem = null;
						SwtichPlayerAction(actionPlayer);//!
					} else
					{
						SwtichPlayerAction(UnitActionPlayer.WANDERING);
					}
				}
			}



			// if need something
			if (actionSystem == null)
			{
				if (IsHungry())
				{
					Debug.Log($"{this} is hungry and go eating");
					SwtichSystemAction(UnitActionSystem.EATING);
				}
				else if (IsTired())
				{
					Debug.Log($"{this} is tired and go sleeping");
					SwtichSystemAction(UnitActionSystem.SLEEPING);
				}
			}
		}

		void OnDrawGizmos()
		{
			float GizAlpha = IsWalking() ? .4f : .9f;
			UnitAction action = GetActualAction();

			if (action == UnitAction.WORKING)
				Gizmos.color = new Color(0f, 0.75f, 0.0f, GizAlpha);
			else if (action == UnitAction.EATING)
				Gizmos.color = new Color(0.75f, 0.75f, 0.0f, GizAlpha);
			else if (action == UnitAction.LEARNING)
				Gizmos.color = new Color(0.0f, 0.75f, 0.75f, GizAlpha);
			else if (action == UnitAction.SLEEPING)
				Gizmos.color = new Color(0.0f, 0.0f, 0.75f, GizAlpha);
			else if (action == UnitAction.WANDERING)
				Gizmos.color = new Color(0.75f, 0.0f, 0.75f, GizAlpha);
			else
				Gizmos.color = new Color(0f, 0f, 0f, GizAlpha);

			Vector3 centerPos = transform.position;
			centerPos.y += 1;
			Gizmos.DrawSphere(centerPos, .25f);
		}
	}
}

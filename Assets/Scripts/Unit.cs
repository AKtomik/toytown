using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
//using System.;

namespace ToyTown
{
	using ActionStartFunction = Action<Unit>;
	using ActionUpdateFunction = Func<Unit, float, ActionUpdateReturn>;

	public enum UnitJob {
		NOTHING = 0,
		FARMER = 1,
		LUMBERJACK = 2,
		MINER = 3,
		BUILDER = 4,
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

	public enum NeedState {
		BEST = 1,
		FINE = 0,
		NEEDED = -1,
		DESPERATION = -2,
		MORTAL = -3,
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
				start: Unit.ActionStartBuilder.Merge(Unit.ActionStartBuilder.JobStart, Unit.ActionStartBuilder.GoingToWork),
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
	[RequireComponent(typeof(BoxCollider))]
	[RequireComponent(typeof(CapsuleCollider))]
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

			public static void JobStart(Unit unit)
			{
				JobsActionStartDictionnary[unit.GetActualJob()](unit);
			}
			
			public static void JobSwitch(Unit unit)
			{
				JobsSwitchDictionnary[unit.GetActualJob()](unit);
			}
			
			public static void GoingToWork(Unit unit)
			{
				Place place = (Place)unit.GetActualJob();
				if (!PlaceManager.Instance.ExistPlace(place, unit.transform.position))
				{
					Debug.Log($"{unit} there is no {place} to work!");
					return;
				}
				unit.walkingObjective = PlaceManager.Instance.GetNearestPlace(place, unit.transform.position);
			}

			// jobs functions
			public static Dictionary<UnitJob, ActionStartFunction> JobsSwitchDictionnary = new()
			{
				{ UnitJob.NOTHING, unit => {
					unit.toolMeshFilterComponent.mesh = unit.RenderOfTools[0].mesh;
					unit.toolMeshRenderComponent.materials = new Material[] { unit.RenderOfTools[0].material };
				} },
				{ UnitJob.FARMER, unit => {
					unit.toolMeshFilterComponent.mesh = unit.RenderOfTools[1].mesh;
					unit.toolMeshRenderComponent.materials = new Material[] { unit.RenderOfTools[1].material };
				} },
				{ UnitJob.LUMBERJACK, unit => {
					unit.toolMeshFilterComponent.mesh = unit.RenderOfTools[2].mesh;
					unit.toolMeshRenderComponent.materials = new Material[] { unit.RenderOfTools[2].material };
				} },
				{ UnitJob.MINER, unit => {
					unit.toolMeshFilterComponent.mesh = unit.RenderOfTools[3].mesh;
					unit.toolMeshRenderComponent.materials = new Material[] { unit.RenderOfTools[3].material };
				} },
				{ UnitJob.BUILDER, unit => {
					unit.toolMeshFilterComponent.mesh = unit.RenderOfTools[4].mesh;
					unit.toolMeshRenderComponent.materials = new Material[] { unit.RenderOfTools[4].material };
				} }
			};
			
			public static Dictionary<UnitJob, ActionStartFunction> JobsActionStartDictionnary = new()
			{
				{ UnitJob.NOTHING, unit => {
					
				} },
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
					if (!PlaceManager.Instance.ExistPlace(place, unit.transform.position))
					{
						Debug.Log($"{unit} there is no {place} to go!");
						unit.hasPlaceToGo = false;
						return;
					}
					unit.hasPlaceToGo = true;
					unit.walkingObjective = PlaceManager.Instance.GetNearestPlace(place, unit.transform.position);
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
				{ UnitJob.NOTHING, (unit, delta) => {
					return ActionUpdateReturn.CONTINUE;
				} },
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

		public static Dictionary<NeedState, Action<Unit>> EnterSleepState = new()
		{
			{NeedState.BEST, (unit) =>
			{
				unit.energyScore = 1;
			}
			},
			{NeedState.FINE, (unit) => {

			}},
			{NeedState.NEEDED, (unit) => {

			}},
			{NeedState.DESPERATION, (unit) => {
				Debug.Log($"{unit} have to sleep");
			}},
			{NeedState.MORTAL, (unit) =>
			{
				Debug.Log($"{unit} die of lack of sleep");
				unit.Kill();
			}
			},
		};
		
		public static Dictionary<NeedState, Action<Unit>> EnterHungerState = new()
		{
			{NeedState.BEST, (unit) =>
			{
				unit.saturationScore = 1;
			}
			},
			{NeedState.FINE, (unit) => {

			}},
			{NeedState.NEEDED, (unit) => {

			}},
			{NeedState.DESPERATION, (unit) => {
				Debug.Log($"{unit} have to eat");
			}},
			{NeedState.MORTAL, (unit) =>
			{
				Debug.Log($"{unit} die of lack of food");
				unit.Kill();
			}
			},
		};

		private Rigidbody RigidBodyComponent;
		private MeshFilter toolMeshFilterComponent;
		private MeshRenderer toolMeshRenderComponent;
		private CapsuleCollider CapsuleColliderValue;
		private BoxCollider BoxColliderValue;
		
		public Renderer ToolObject;
		public Renderer childRender;
		public Renderer adultRender;
		public List<Material> UnitMaterials = new();
		[System.Serializable]
		public struct ToolRender
		{
			public Mesh mesh;
			public Material material;
		}
		public List<ToolRender> RenderOfTools = new();
		

		public double saturationScore = 1;
		public double energyScore = 1;
		public double happynessScore = .5;
		public NeedState needStateHunger;
		public NeedState needStateSleep;

		public double age = 0;
		public double adultAge = 0;
		public bool spawnAdult = false;
		public bool isAdult { get { return age > adultAge; } }
		
		private UnitActionPlayer actionPlayer = UnitActionPlayer.WANDERING;
		private UnitActionSystem? actionSystem = null;
		private double actionSystemDaysAmount = .0;
		private double actionSystemDaysRemain = .0;
		private bool hasPlaceToGo;
		private Vector3? walkingObjective = null;

		public UnitJob startingJob = UnitJob.NOTHING;
		private UnitJob actualJob = UnitJob.NOTHING;
		public bool isDying { get; private set; }
		public bool isGrabed { get; private set; }
		
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
			ActionStartBuilder.JobSwitch(this);
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
		
		private void GrowingUp()
		{
			childRender.gameObject.SetActive(false);
			ToolObject.gameObject.SetActive(true);
			adultRender.gameObject.SetActive(true);
			Debug.Log($"{this} is growing up and can work");
		}

		public bool IsHungry()
		{
			return needStateHunger < NeedState.FINE;
		}
		
		public bool IsTired()
		{
			return needStateSleep < NeedState.FINE;
		}

		public bool IsWalking()
		{
			return walkingObjective != null && Vector3.Distance((Vector3)walkingObjective, transform.position) > Settings.WalkingNearObjectiveDistance;
		}
		
		static public NeedState CalculateNeedState(double score)
		{
			if (!(score > Settings.UnitNeedPointMortal))
				return NeedState.MORTAL;
			else if (!(score > Settings.UnitNeedPointDesperation))
				return NeedState.DESPERATION;
			else if (!(score > Settings.UnitNeedPointNeeded))
				return NeedState.NEEDED;
			else if (!(score < Settings.UnitMaxNeedPoint))
				return NeedState.BEST;
			else
				return NeedState.FINE;
		}
		
		public double speed
		{
			get {
				return Settings.UnitBaseSpeed * Settings.SpeedUp * (IsHungry() ? .5 : 1) * (IsTired() ? .5 : 1);
			}
		}

		// Start is called once before the first execution of Update after the MonoBehaviour is created
		void Start()
		{
			RigidBodyComponent = GetComponent<Rigidbody>();
			toolMeshFilterComponent = ToolObject.GetComponent<MeshFilter>();
			toolMeshRenderComponent = ToolObject.GetComponent<MeshRenderer>();
			BoxColliderValue = GetComponent<BoxCollider>();
			if (BoxColliderValue == null) Debug.LogError($"BoxCollider ref is null! [{BoxColliderValue}]");
			CapsuleColliderValue = GetComponent<CapsuleCollider>();
			if (CapsuleColliderValue == null) Debug.LogError($"CapsuleCollider ref is null! [{CapsuleColliderValue}]");
			if (!Action.Dictionnary.Keys.Contains((UnitAction)this.actionPlayer))
			{
				Debug.LogError($"actionPlayer is not a correct UnitAction! Please choose a value for {this}.actionPlayer. (this.actionPlayer = {this.actionPlayer})");
				this.actionPlayer = UnitActionPlayer.WANDERING;
			}
			// init
			Material colorMaterial = UnitMaterials.ElementAt(Random.Range(0, UnitMaterials.Count));
			if (colorMaterial == null) Debug.LogError($"Material not found!");
			if (!childRender.TryGetComponent<MeshRenderer>(out var childMesh)) Debug.LogError($"no childMesh! {childMesh}");
			childMesh.materials = new Material[] { colorMaterial };
			if (!adultRender.TryGetComponent<MeshRenderer>(out var adultMesh)) Debug.LogError($"no adultMesh! {adultMesh}");
			adultMesh.materials = new Material[] { colorMaterial };

			// random age
			if (spawnAdult)
			{
				GrowingUp();
			} else
			{
				adultAge = Settings.UnitAdultAgeMin + Random.value * (Settings.UnitAdultAgeMax - Settings.UnitAdultAgeMin);
				Debug.Log($"{this} will grow up in {adultAge} days!");
			}

			// ! test
			Debug.Log($"switch job to startingJob {startingJob}");
			SwtichJob(startingJob);
			SwtichPlayerAction(UnitActionPlayer.WORKING);
		}

		// Update is called once per frame
		void Update()
		{
			// growing up
			bool wasAdult = isAdult;
			age += Time.deltaTime * speed / Settings.DayLengthInSecond;
			if (!wasAdult && isAdult)
			{
				GrowingUp();
			}

			// if walking
			if (!hasPlaceToGo)
			{
				return;
			}
			if (IsWalking())
			{
				RigidBodyComponent.MovePosition(Vector3.MoveTowards(transform.position, (Vector3)walkingObjective, (float)(Time.deltaTime * speed * Settings.WalkingSpeed)));
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

			NeedState sleepNeed = CalculateNeedState(energyScore);
			if (sleepNeed != needStateSleep)
			{
				EnterSleepState[sleepNeed](this);
				needStateSleep = sleepNeed;
			}

			NeedState hungerNeed = CalculateNeedState(saturationScore);
			if (hungerNeed != needStateHunger)
			{
				EnterHungerState[hungerNeed](this);
				needStateHunger = hungerNeed;
			}

			if (happynessScore > 1) happynessScore = 1;
			else if (happynessScore < 0) happynessScore = 0;

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

		void Kill()
		{
			isDying = true;
			Destroy(gameObject);
		}
		
		public void Grab()
		{
			BoxColliderValue = GetComponent<BoxCollider>();
			BoxColliderValue.enabled = false;
			CapsuleColliderValue = GetComponent<CapsuleCollider>();
			CapsuleColliderValue.enabled = false;
			RigidBodyComponent.useGravity = false;
			RigidBodyComponent.isKinematic = true;
			isGrabed = true;
		}
		
		public void Release()
		{
			BoxColliderValue = GetComponent<BoxCollider>();
			BoxColliderValue.enabled = true;
			CapsuleColliderValue = GetComponent<CapsuleCollider>();
			CapsuleColliderValue.enabled = true;
			RigidBodyComponent.useGravity = true;
			RigidBodyComponent.isKinematic = false;
			isGrabed = false;
			Place? GroundPlace = PlaceManager.Instance.GetTilePlace(transform.position);
			if (!GroundPlace.HasValue) return;
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
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
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
					unit.hasPlaceToGo = false;
					Debug.Log($"{unit} there is no {place} to work!");
					return;
				}
				unit.hasPlaceToGo = true;
				Debug.Log($"{unit} is going to work {place} ({PlaceManager.Instance})");
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
					Debug.Log($"{unit} is going to place {place} ({PlaceManager.Instance})");
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
				if (unit.learningJob == UnitJob.NOTHING) return ActionUpdateReturn.CONTINUE;
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
					unit.miningProgress += Settings.MiningFoodByDay * delta / Settings.DayLengthInSecond;
					if (unit.miningProgress > 1)
					{
						unit.miningProgress -= 1;
						RessourcesGestion.AddFood();
					}
					return ActionUpdateReturn.CONTINUE;
				} },
				{ UnitJob.LUMBERJACK, (unit, delta) => {
					unit.miningProgress += Settings.MiningWoodByDay * delta / Settings.DayLengthInSecond;
					if (unit.miningProgress > 1)
					{
						unit.miningProgress -= 1;
						RessourcesGestion.AddWood();
					}
					return ActionUpdateReturn.CONTINUE;
				} },
				{ UnitJob.MINER, (unit, delta) => {
					unit.miningProgress += Settings.MiningStoneByDay * delta / Settings.DayLengthInSecond;
					if (unit.miningProgress > 1)
					{
						unit.miningProgress -= 1;
						RessourcesGestion.AddRock();
					}
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
		public double deathAge = 0;
		public bool spawnAdult = false;
		public bool isAdult { get { return age > adultAge; } }
		public bool isTooOld { get { return age > deathAge; } }
		
		private UnitActionPlayer actionPlayer = UnitActionPlayer.WANDERING;
		private UnitActionSystem? actionSystem = null;
		private double actionSystemDaysAmount = .0;
		private double actionSystemDaysRemain = .0;
		private bool hasPlaceToGo;
		private double noPlaceRethinking = 0;
		private bool walkingWondering;
		private Vector3? walkingObjective = null;

		public UnitJob startingJob = UnitJob.NOTHING;
		private UnitJob actualJob = UnitJob.NOTHING;
		private double miningProgress = 0;
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
			walkingWondering = false;
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
			walkingWondering = false;
			Action.Dictionnary[(UnitAction)action].Start(this);
			actionPlayer = action;
		}

		public void ReswitchAction()
		{
			if (actionSystem != null)
				SwtichSystemAction(actionSystem.Value);
			else
				SwtichPlayerAction(actionPlayer);
		}
		
		public void StartLearning(UnitJob job)
		{
			if (!isAdult && !Settings.ChildLearnable && job != UnitJob.NOTHING)
			{
				Debug.Log($"{this} cant learn");	
				return;
			}
			learningJob = job;
			SwtichPlayerAction(UnitActionPlayer.LEARNING);
		}
		
		public void SwtichJob(UnitJob job)
		{
			if (!isAdult && !Settings.ChildLabour && job != UnitJob.NOTHING)
			{
				Debug.Log($"{this} cant work bcs is a child");	
				return;
			}
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

		public bool IsWaitingLearningJob()
		{
			return GetActualAction() == UnitAction.LEARNING && learningJob == UnitJob.NOTHING;
		}
		
		public void SelectLearningJob(UnitJob job)
		{
			learningJob = job;
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

		public bool wasWalking = false;
		public bool IsWalking()
		{
			if (isGrabed) return wasWalking;
			wasWalking = walkingObjective != null && Vector3.Distance((Vector3)walkingObjective, transform.position) > Settings.WalkingNearObjectiveDistance;
			return wasWalking;
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
				return Settings.UnitBaseSpeed * Settings.SpeedUp * (IsHungry() ? .5 : 1) * (IsTired() ? .5 : 1) * (walkingWondering ? .5 : 1);
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

			// random age and death
			float randomLater = Random.value;
			deathAge = Math.Round(Settings.UnitDeathAgeMin + randomLater * (Settings.UnitDeathAgeMax - Settings.UnitDeathAgeMin));
				Debug.Log($"{this} will die in {deathAge} days!");
			if (spawnAdult)
			{
				GrowingUp();
			} else
			{
				SwtichJob(UnitJob.NOTHING);
				SwtichPlayerAction(UnitActionPlayer.WANDERING);
				adultAge = Math.Round(Settings.UnitAdultAgeMin + randomLater * (Settings.UnitAdultAgeMax - Settings.UnitAdultAgeMin));
				Debug.Log($"{this} will grow up in {adultAge} days!");
			}
		}

		int tickCount = 0;
		public int selfCount = 0;
		void FirstTick()
		{
			selfCount++;
			Debug.Log($"switch job to startingJob {startingJob}");
			SwtichJob(startingJob);
			SwtichPlayerAction(UnitActionPlayer.WORKING);
		}

		// Update is called once per frame
		void Update()
		{
			tickCount += 1;
			if (tickCount == 2)
			{
				FirstTick();
			}

			// growing up
			bool wasAdult = isAdult;
			double spendTime = Time.deltaTime * speed / Settings.DayLengthInSecond;
			age += spendTime;
			if (!wasAdult && isAdult)
			{
				GrowingUp();
			}
			if (!isDying && isTooOld)
			{
				Kill();
			}

			if (isGrabed) return;

			// if walking
			if (!hasPlaceToGo)
			{
				// rethinking
				noPlaceRethinking += spendTime;
				if (noPlaceRethinking > Settings.RethinkingTimeDay)
				{
					noPlaceRethinking = 0;
					ReswitchAction();
				}
				// wondering
				if (!IsWalking())
				{
					walkingObjective = PlaceManager.Instance.RandomPlace();
				}
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
			RigidBodyComponent = GetComponent<Rigidbody>();
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
			RigidBodyComponent = GetComponent<Rigidbody>();
			RigidBodyComponent.useGravity = true;
			RigidBodyComponent.isKinematic = false;
			isGrabed = false;

			Place? GroundPlace = PlaceManager.Instance.GetTilePlace(transform.position);
			if (!GroundPlace.HasValue || GroundPlace == Place.POINT) return;
			Debug.Log($"FALL ON Place [{GroundPlace}]");

			switch (GroundPlace.Value)
			{

				case Place.FARM:
				case Place.BUSH:
					{
						SwtichJob(UnitJob.FARMER);
						SwtichPlayerAction(UnitActionPlayer.WORKING);
					} break;
				case Place.CONSTRUCTION:
					{
						SwtichJob(UnitJob.BUILDER);
						SwtichPlayerAction(UnitActionPlayer.WORKING);
					} break;
				case Place.WOOD:
					{
						SwtichJob(UnitJob.LUMBERJACK);
						SwtichPlayerAction(UnitActionPlayer.WORKING);
					} break;
				case Place.MINE:
					{
						SwtichJob(UnitJob.MINER);
						SwtichPlayerAction(UnitActionPlayer.WORKING);
					} break;
					
				case Place.CANTINE:
					{
						SwtichSystemAction(UnitActionSystem.EATING);
					} break;
				case Place.HOUSE:
					{
						SwtichSystemAction(UnitActionSystem.SLEEPING);
					} break;
					
				case Place.SCHOOL:
					{
						StartLearning(UnitJob.NOTHING);
					} break;
				
				default:
					{
						
					} break;
			}
		}

		void OnDrawGizmos()
		{
			float GizAlpha = (IsWalking() || !hasPlaceToGo && Math.Round(age * 1000) % 2 == 0) ? .4f : .9f;
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
using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour 
{
    // current debugging level for this script
    private readonly DEBUG_LEVEL UNITY_DEBUG = DEBUG_LEVEL.GENERAL;

	public static SeasonManager.Season currSeason;

    // Illegal location for character movement
    private readonly Vector3 INVALID_LOCATION = new Vector3(0.0f, 0.0f, -1.0f);
    // Maximum amount of satiety a character can have
    private const ushort FULLNESS_LEVEL_MAX     = 100;
    // Maximum amount of heat a character can have
    private const ushort HEAT_LEVEL_MAX         = 100;
    // Maximum amount of health a character can have
    private const ushort HEALTH_LEVEL_MAX       = 100;
    // Amount of time it takes to become an adult (in seconds)
    private const float TIME_UNTIL_ADULT        = SeasonManager.seasonLength * 2.0f;
    // Amount of time it takes to become an elder (in seconds)
    private const float TIME_UNTIL_ELDER        = TIME_UNTIL_ADULT + (SeasonManager.seasonLength * 4.0f);
    // Amount of time it takes to die (in seconds)
    private const float TIME_UNTIL_DEATH        = TIME_UNTIL_ELDER + (SeasonManager.seasonLength * 2.0f);
    // Amount of time it takes to be able to mate again
    private const float TIME_UNTIL_NEXT_MATE    = ((float)SeasonManager.seasonLength * 1.0f);
    // Amount of heat gained (per second) from a fireplace
    private const float FIREPLACE_HEAT_GAIN     = 10;

    // The levels of debugging for this script
    // Note: Levels are incremental
    private enum DEBUG_LEVEL
    {
        OFF     = 0, // No debugging
        ERROR   = 1, // Only errors
        WARNING = 2, // Warnings
        GENERAL = 3, // Useful events for debugging
        VERBOSE = 4  // Log most things
    };

    // Motion states of the character
    public enum CHARACTER_MOTION_STATE
    {
        IDLE            = 0,
        MOVING          = 1, 
        INVALID         = 2
    };

    // Action states of the character
    public enum CHARACTER_ACTION_STATE
    {
        IDLE            = 0, // Gathering a resource
        CARRYING_FOOD   = 1, // Carrying food
        CARRYING_WOOD   = 2, // Carrying wood
        DROPOFF_FOOD    = 3, // Dropping off food
        DROPOFF_WOOD    = 4, // Dropping off wood
        CONSUME_FOOD    = 5, // Consuming food
        WAITING         = 6, // Waiting to mate
        MATING          = 7, // Mating
        INVALID         = 8
    };

    // Ages of the character
    public enum CHARACTER_AGE
    {
        YOUNG   = 0,
        ADULT   = 1,
        ELDER   = 2,
        DEATH   = 3,
        INVALID = 4
    };

    // Genders of the character
    public enum CHARACTER_GENDER
    {
        MALE    = 0,
        FEMALE  = 1
    };

    // Speeds of the character
    // NOTE: Get 10% of this number!
    public enum CHARACTER_SPEED
    {
        YOUNG = 7,
        ADULT = 5,
        ELDER = 3,
    };

    // Food gathering rates of the character
    public enum CHARACTER_FOOD_GATHER
    {
        YOUNG = 3,
        ADULT = 5,
        ELDER = 3,
        DEATH = 0
    };

    // Food consuming rates of the food
    public enum CHARACTER_FOOD_CONSUME
    {
        YOUNG = 5,
        ADULT = 3,
        ELDER = 1,
        DEATH = 0
    };

    // Wood gathering rates of the character
    public enum CHARACTER_WOOD_GATHER
    {
        YOUNG = 0,
        ADULT = 3,
        ELDER = 1,
        DEATH = 0
    };

    // Objective of the character
    public enum CHARACTER_OBJECTIVE
    {
        NONE            = 0,
        FOOD_GATHER     = 1, // Going to gather food
        FOOD_CONSUME    = 2, // Going to consume food
        FOOD_DROPOFF    = 3, // Going to drop off food
        WOOD_GATHER     = 4, // Going to gather wood
        WOOD_DROPOFF    = 6, // Going to drop off wood
        MATE            = 7, // Going to mate tree
        KINDLE          = 8, // Going to turn on fire
        INVALID         = 9
    };

    /// <summary>
    /// Rate at which the character loses heat
    /// </summary>
    public enum CHARACTER_FREEZE_RATE
    {
        YOUNG = 4,
        ADULT = 2,
        ELDER = 6,
        DEATH = 0
    }

    /// <summary>
    /// Rate at which the character loses fullness
    /// </summary>
    public enum CHARACTER_STARVATION_RATE
    {
        YOUNG = 3,
        ADULT = 1,
        ELDER = 2,
        DEATH = 0
    }

    /// <summary>
    /// Rate at which a complete freeze decreases health
    /// </summary>
    public enum CHARACTER_FREEZE_HEALTH_DECLINE_RATE
    {
        YOUNG = 6,
        ADULT = 2,
        ELDER = 4,
        DEATH = 0
    }

    /// <summary>
    /// Rate at which complete starvation decreases health
    /// </summary>
    public enum CHARACTER_STARVATION_HEALTH_DECLINE_RATE
    {
        YOUNG = 4,
        ADULT = 2,
        ELDER = 6,
        DEATH = 0
    }

    // Target position of the character
    private Vector3 m_targetPos;
    // Sprite for the character
    private SpriteRenderer spriteRenderer;
	private CharacterAnimationScript animationScript;

	public GameObject carryingFoodIcon;
	public GameObject carryingWoodIcon;

    // Amount of a resource that the character is carrying
    private ushort m_amountCarrying;
    // Amount of time remaining until the character can mate
    private float m_timeUntilNextMate;
    // Time at which the character was created
    private float m_timeCreated;
    // How full the character is currently
    private float m_fullnessLevel;
    // How heated the character is currently
    private float m_heatLevel;
    // Current health of the character
    private float m_health;
    // If the character is by a fireplace
    private bool m_isToastingMarshmallows;

    // Gender of the character
    private CHARACTER_GENDER m_gender;
    // Age of the character
    private CHARACTER_AGE m_age;
    // Current motion state of the character
    private CHARACTER_MOTION_STATE m_motion_state;
    // Current action state of the character
    private CHARACTER_ACTION_STATE m_action_state;
    // Current speed of the character
    private CHARACTER_SPEED m_speed;
    // Food consume rate of the character
    private CHARACTER_FOOD_CONSUME m_foodConsume;
    // Food gather rate of the character
    private CHARACTER_FOOD_GATHER m_foodGather;
    // Wood gather rate of the character
    private CHARACTER_WOOD_GATHER m_woodGather;
    // Objective of the character
    private CHARACTER_OBJECTIVE m_objective;
    // Current freeze rate of the character
    private CHARACTER_FREEZE_RATE m_freezeRate;
    // Current starvation rate of the character
    private CHARACTER_STARVATION_RATE m_starvationRate;
    // Current health decline rate from freezing of the character
    private CHARACTER_FREEZE_HEALTH_DECLINE_RATE m_freezeHealthDeclineRate;
    // Current health decline rate from starvation of the character
    private CHARACTER_STARVATION_HEALTH_DECLINE_RATE m_starvationHealthDeclineRate;

    /// <summary>
    /// Fullness of the character
    /// </summary>
    public float FullnessLevel
    {
        get { return m_fullnessLevel; }
    }

    /// <summary>
    /// Heat level of the character
    /// </summary>
    public float HeatLevel
    {
        get { return m_heatLevel; }
    }

    /// <summary>
    /// Health of the character
    /// </summary>
    public float Health
    {
        get { return m_health; }
    }

    /// <summary>
    /// The amount of time that has elapsed since the creation of this character
    /// </summary>
    public float TimeElapsed
    {
        get { return Time.time - m_timeCreated; }
    }

    /// <summary>
    /// Gender of the character
    /// </summary>
    public CHARACTER_GENDER Gender
    {
        get { return m_gender; }
    }

    /// <summary>
    /// Age of the character
    /// </summary>
    public CHARACTER_AGE Age
    {
        get { return m_age; }
    }

    /// <summary>
    /// Current motion state of the character
    /// </summary>
    public CHARACTER_MOTION_STATE MotionState
    {
        get { return m_motion_state; }
        set { m_motion_state = value; }
    }

    /// <summary>
    /// Current action state of the character
    /// </summary>
    public CHARACTER_ACTION_STATE ActionState
    {
        get { return m_action_state; }
        set { m_action_state = value; }
    }

    /// <summary>
    /// Objective of the character
    /// </summary>
    public CHARACTER_OBJECTIVE Objective
    {
        get { return m_objective; }
    }

    /// <summary>
    /// Amount of food the character consumes
    /// </summary>
    public CHARACTER_FOOD_CONSUME FoodConsumeAmount
    {
        get { return m_foodConsume; }
    }

    /// <summary>
    /// Amount of food the character gathers
    /// </summary>
    public CHARACTER_FOOD_GATHER FoodGatherAmount
    {
        get { return m_foodGather; }
    }

    /// <summary>
    /// Amount of wood the character gathers
    /// </summary>
    public CHARACTER_WOOD_GATHER WoodGatherAmount
    {
        get { return m_woodGather; }
    }

    /// <summary>
    /// Returns whether or not the character can gather wood
    /// </summary>
    /// <returns>If the character can gather wood</returns>
    public bool CanGatherWood()
    {
        return (
            (CHARACTER_AGE.YOUNG != m_age &&
             CHARACTER_AGE.DEATH != m_age) &&
            (CHARACTER_MOTION_STATE.IDLE == m_motion_state ||
             CHARACTER_MOTION_STATE.MOVING == m_motion_state));                    
    }

    /// <summary>
    /// Returns whether or not the character can gather food
    /// </summary>
    /// <returns>If the character can gather food</returns>
    public bool CanGatherFood()
    {
        return (
            (CHARACTER_AGE.DEATH != m_age) &&
            (CHARACTER_MOTION_STATE.IDLE == m_motion_state ||
             CHARACTER_MOTION_STATE.MOVING == m_motion_state));
    }

    /// <summary>
    /// Returns whether or not the character can mate
    /// </summary>
    /// <returns>If the character can mate</returns>
    public bool CanMate()
    {
        return (CHARACTER_AGE.ADULT == m_age && m_timeUntilNextMate <= 0);
    }

	/// <summary>
	/// Initial character script creation
	/// </summary>
	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationScript = GetComponent<CharacterAnimationScript>();
	}

    /// <summary>
    /// Initializes the character with defaults along with the given parameters
    /// </summary>
    /// <param name="gender">Gender for the character</param>
    /// <param name="age">[Optional] Age for the character</param>
    public void Init(CharacterScript.CHARACTER_GENDER gender, CharacterScript.CHARACTER_AGE age = CHARACTER_AGE.INVALID)
    {
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationScript = GetComponent<CharacterAnimationScript>();
		animationScript.Init();
        // Set character age if provided
        if (CHARACTER_AGE.INVALID != age)
        {
            m_age = age;

            // Set age-sensitive variables
            switch (m_age)
            {
                case CHARACTER_AGE.YOUNG:
                {
                    m_foodConsume                   = CHARACTER_FOOD_CONSUME.YOUNG;
                    m_foodGather                    = CHARACTER_FOOD_GATHER.YOUNG;
                    m_woodGather                    = CHARACTER_WOOD_GATHER.YOUNG;
                    m_speed                         = CHARACTER_SPEED.YOUNG;
                    m_freezeRate                    = CHARACTER_FREEZE_RATE.YOUNG;
                    m_starvationRate                = CHARACTER_STARVATION_RATE.YOUNG;
                    m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.YOUNG;
                    m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.YOUNG;

					animationScript.SetToYoung();
                    break;
                }

                case CHARACTER_AGE.ADULT:
                {
                    m_foodConsume                   = CHARACTER_FOOD_CONSUME.ADULT;
                    m_foodGather                    = CHARACTER_FOOD_GATHER.ADULT;
                    m_woodGather                    = CHARACTER_WOOD_GATHER.ADULT;
                    m_speed                         = CHARACTER_SPEED.ADULT;
                    m_freezeRate                    = CHARACTER_FREEZE_RATE.ADULT;
                    m_starvationRate                = CHARACTER_STARVATION_RATE.ADULT;
                    m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.ADULT;
                    m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.ADULT;

					animationScript.SetToAdult();
					break;
                }

                case CHARACTER_AGE.ELDER:
                {
                    m_foodConsume                   = CHARACTER_FOOD_CONSUME.ELDER;
                    m_foodGather                    = CHARACTER_FOOD_GATHER.ELDER;
                    m_woodGather                    = CHARACTER_WOOD_GATHER.ELDER;
                    m_speed                         = CHARACTER_SPEED.ELDER;
                    m_freezeRate                    = CHARACTER_FREEZE_RATE.ELDER;
                    m_starvationRate                = CHARACTER_STARVATION_RATE.ELDER;
                    m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.ELDER;
                    m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.ELDER;

					animationScript.SetToOld();
                    break;
                }

                default:
                {
                    if(DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                        Debug.Log("This shouldn't happen! CharacterScript::Init()");

                    break;
                }
            }
        }

        // Age not provided - use defaults
        else
        {
            m_age = CHARACTER_AGE.YOUNG;

            // Set age-sensitive variables
            m_foodConsume                   = CHARACTER_FOOD_CONSUME.YOUNG;
            m_foodGather                    = CHARACTER_FOOD_GATHER.YOUNG;
            m_woodGather                    = CHARACTER_WOOD_GATHER.YOUNG;
            m_speed                         = CHARACTER_SPEED.YOUNG;
            m_freezeRate                    = CHARACTER_FREEZE_RATE.YOUNG;
            m_starvationRate                = CHARACTER_STARVATION_RATE.YOUNG;
            m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.YOUNG;
            m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.YOUNG;

			Debug.Log(animationScript);
			animationScript.SetToYoung();
        }

        // set remaining variables
        m_fullnessLevel = FULLNESS_LEVEL_MAX;
        m_heatLevel     = HEAT_LEVEL_MAX;
        m_health        = HEALTH_LEVEL_MAX;

        m_timeCreated               = Time.time;
        m_amountCarrying            = 0;
        m_isToastingMarshmallows    = false;

        // set remaining enums
        m_gender            = gender;
        m_objective         = CHARACTER_OBJECTIVE.NONE;
        m_motion_state      = CHARACTER_MOTION_STATE.IDLE;
        m_action_state      = CHARACTER_ACTION_STATE.IDLE;
        m_targetPos         = INVALID_LOCATION;
        m_timeUntilNextMate = 0;

		animationScript.SetGender(gender == CHARACTER_GENDER.MALE);
    }

    /// <summary>
    /// Updates the character
    /// </summary>
    void Update()
    {
        UpdateAge();
        UpdateAttributes();
        SetSortingOrder();

        // Update mating timer
        m_timeUntilNextMate -= Time.deltaTime;

        if (0 >= m_timeUntilNextMate)
        {
            m_timeUntilNextMate = 0;

            if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG &&
                CHARACTER_AGE.ADULT == m_age)
                Debug.Log("Available to mate!");
        }

        // Character functionality is only applicable while alive
        if (CHARACTER_AGE.DEATH != m_age)
        {
            // Update position
            if (INVALID_LOCATION != m_targetPos)
            {
                MoveToTarget();
            }
        }

        if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG)
            Debug.Log("Action state: " + m_action_state);

        if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG)
            Debug.Log("Motion state: " + m_motion_state);
    }

    /// <summary>
    /// Updates the various attributes for the character
    /// </summary>
    void UpdateAttributes()
    {
        // Freezing ------------------------------------------

        // Lose heat if not by a fire
        if (currSeason == SeasonManager.Season.WINTER &&
		    (!m_isToastingMarshmallows || !ResourceManager.IsFireLit()))
        {
            m_heatLevel -= (float)m_freezeRate * Time.deltaTime;

            if(0 >= m_heatLevel)
            {
                m_heatLevel = 0;
                
                if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                    Debug.Log("Lost all heat!");
            }

            if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG)
                Debug.Log("lost heat! Now at " + m_heatLevel + " heat!");
        }

        // Gain heat if by a fire
		else
		{
			m_heatLevel += FIREPLACE_HEAT_GAIN * Time.deltaTime;

            if (m_heatLevel >= HEAT_LEVEL_MAX)
                m_heatLevel = HEAT_LEVEL_MAX;

            if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG)
                Debug.Log("gained heat! Now at " + m_heatLevel + " heat!");
        }

        // Starving ----------------------------------------------

        // Lose fullness constantly
		m_fullnessLevel -= (float)m_starvationRate * Time.deltaTime;

        if (DEBUG_LEVEL.VERBOSE <= UNITY_DEBUG)
            Debug.Log("Lost fullness! Now at " + m_fullnessLevel + " fullness!");

        if (0 >= m_fullnessLevel)
            m_fullnessLevel = 0;

        // Health -------------------------------------------------
        
        // Starvation health decline
        if (0 >= m_fullnessLevel)
			m_health -= (float)m_starvationHealthDeclineRate * Time.deltaTime;

        if (0 >= m_heatLevel)
			m_health -= (float)m_freezeHealthDeclineRate * Time.deltaTime;

        // Death when health reaches 0
        if (0 >= m_health && m_age != CHARACTER_AGE.DEATH)
		{
            m_age = CHARACTER_AGE.DEATH;
			animationScript.SetToDeath();
		}
    }

    /// <summary>
    /// Updates the age of the character based on time elapsed since its creation
    /// </summary>
    void UpdateAge()
    {
        // Change from young -> adult if required time has elapsed
        if (TIME_UNTIL_ADULT <= (Time.time - m_timeCreated) &&
            CHARACTER_AGE.YOUNG == m_age)
        {
            m_age                           = CHARACTER_AGE.ADULT;
            m_foodConsume                   = CHARACTER_FOOD_CONSUME.ADULT;
            m_foodGather                    = CHARACTER_FOOD_GATHER.ADULT;
            m_woodGather                    = CHARACTER_WOOD_GATHER.ADULT;
            m_freezeRate                    = CHARACTER_FREEZE_RATE.ADULT;
            m_starvationRate                = CHARACTER_STARVATION_RATE.ADULT;
            m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.ADULT;
            m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.ADULT;

            if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Change to adult");

			animationScript.SetToAdult();
        }

        // Change from adult -> elder if required time has elapsed
        else if (TIME_UNTIL_ELDER <= (Time.time - m_timeCreated) &&
                CHARACTER_AGE.ADULT == m_age)
        {
            m_age                           = CHARACTER_AGE.ELDER;
            m_foodConsume                   = CHARACTER_FOOD_CONSUME.ELDER;
            m_foodGather                    = CHARACTER_FOOD_GATHER.ELDER;
            m_woodGather                    = CHARACTER_WOOD_GATHER.ELDER;
            m_freezeRate                    = CHARACTER_FREEZE_RATE.ELDER;
            m_starvationRate                = CHARACTER_STARVATION_RATE.ELDER;
            m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.ELDER;
            m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.ELDER;

            if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Change to elder");

			animationScript.SetToOld();
        }

        // Change from elder -> death if required time has elapsed
        else if (TIME_UNTIL_DEATH <= (Time.time - m_timeCreated) &&
                CHARACTER_AGE.ELDER == m_age)
        {
            m_age                           = CHARACTER_AGE.DEATH;
            m_foodConsume                   = CHARACTER_FOOD_CONSUME.DEATH;
            m_foodGather                    = CHARACTER_FOOD_GATHER.DEATH;
            m_woodGather                    = CHARACTER_WOOD_GATHER.DEATH;
            m_freezeRate                    = CHARACTER_FREEZE_RATE.DEATH;
            m_starvationRate                = CHARACTER_STARVATION_RATE.DEATH;
            m_freezeHealthDeclineRate       = CHARACTER_FREEZE_HEALTH_DECLINE_RATE.DEATH;
            m_starvationHealthDeclineRate   = CHARACTER_STARVATION_HEALTH_DECLINE_RATE.DEATH;
            
            if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Change to death");

			animationScript.SetToDeath();
        }
    }

    /// <summary>
    /// Changes the current fullness level of the character by the specified amount
    /// </summary>
    /// <param name="amnt">Amount that the fullness level will be modified</param>
    public void ChangeFullnessLevel(short amnt)
    {
        m_fullnessLevel += (ushort) amnt;

        // boundary checks -------------------
        if (FULLNESS_LEVEL_MAX <= m_fullnessLevel)
            m_fullnessLevel = FULLNESS_LEVEL_MAX;

        else if (0 >= m_fullnessLevel)
            m_fullnessLevel = 0;
    }

    /// <summary>
    /// Changes the current heat level of the character by the specified
    /// </summary>
    /// <param name="amnt">Amount that the heat level will be modified</param>
    public void ChangeHeatLevel(short amnt)
    {
        m_heatLevel += (ushort)amnt;

        // boundary checks ---------------------
        if (HEAT_LEVEL_MAX <= m_heatLevel)
            m_heatLevel = HEAT_LEVEL_MAX;

        else if (m_heatLevel <= 0)
            m_heatLevel = 0;
    }

    /// <summary>
    /// Character goes back to being idle once it is done mating
    /// </summary>
    public void DoneMating()
    {
        // Update states and variables
        m_timeUntilNextMate = TIME_UNTIL_NEXT_MATE;

        m_objective     = CHARACTER_OBJECTIVE.NONE;
        m_motion_state  = CHARACTER_MOTION_STATE.IDLE;
        m_action_state  = CHARACTER_ACTION_STATE.IDLE;
    }

    /// <summary>
    /// Character begins to mate
    /// </summary>
    public void StartMating()
    {
        m_objective     = CHARACTER_OBJECTIVE.MATE;
        m_motion_state  = CHARACTER_MOTION_STATE.IDLE;
        m_action_state  = CHARACTER_ACTION_STATE.MATING;
    }

    /// <summary>
    /// Moves the character to the target position, and sets its new objective if necessary
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="objective"></param>
	public void SetTargetPos(Vector3 pos, CharacterScript.CHARACTER_OBJECTIVE objective = CHARACTER_OBJECTIVE.INVALID)
	{
        // Can only get a new target if it's not mating
        if (CHARACTER_ACTION_STATE.MATING != m_action_state)
        {
            m_targetPos = pos;
            m_targetPos.z = 0;
			animationScript.SetToMoving();
			animationScript.SetFacing(transform.position.x, pos.x);
        }

        else
            m_targetPos = INVALID_LOCATION;

	
        if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
            Debug.Log("Target pos: " + m_targetPos);

        // Set objective if necessary
        if (CHARACTER_OBJECTIVE.INVALID != objective)
            m_objective = objective;
        else
            m_objective = CHARACTER_OBJECTIVE.NONE;

        // Only set character to moving if they aren't already moving
        if (CHARACTER_MOTION_STATE.IDLE != m_motion_state ||
            CHARACTER_MOTION_STATE.INVALID != m_motion_state)
        {
            m_motion_state = CHARACTER_MOTION_STATE.MOVING;
        }
	}

    /// <summary>
    /// Moves the character to its target
    /// </summary>
	private void MoveToTarget()
	{
        // Have we reached our target?
        if (Vector3.Distance(transform.position, m_targetPos) < 0.01f)
        {
			animationScript.SetToIdle();
            if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Reached dest!");

            // Update motion state
            m_motion_state = CHARACTER_MOTION_STATE.IDLE;

            // Update target
            m_targetPos = INVALID_LOCATION;

            // Check objectives
            switch (m_objective)
            {
                case CHARACTER_OBJECTIVE.NONE:
                {
                    if(DEBUG_LEVEL.WARNING <= UNITY_DEBUG)
                        Debug.Log("Had no objective!");

                    break;
                }

                // Gathering food 
                case CHARACTER_OBJECTIVE.FOOD_GATHER:
                {
                    // Only gather if able
                    if (CanGatherFood())
                    {
                        // Update states and variables
                        m_amountCarrying = (ushort)m_foodGather;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.CARRYING_FOOD;

                        if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                        {
                            Debug.Log("Food gathered!");
                            Debug.Log("Amount carrying: " + m_amountCarrying);
                        }

						carryingFoodIcon.SetActive(true);
                    }

                    else
                    {
                        // Update states and variables
                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if (DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                            Debug.Log("Trying to gather food but can't!");
                    }
                    
                    break;
                }

                // Dropping off food
                case CHARACTER_OBJECTIVE.FOOD_DROPOFF:
                {
                    // Only drop off food if the character is carrying food
                    if (CHARACTER_ACTION_STATE.CARRYING_FOOD == m_action_state)
                    {
                        // Update food stockpile
                        ResourceManager.ChangeFood(m_amountCarrying);

                        // Update states and variables
                        m_amountCarrying = 0;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                        {
                            Debug.Log("Dropped off food!");
                            Debug.Log("Amount carrying: " + m_amountCarrying);
                        }
                        
						carryingFoodIcon.SetActive(false);
                    }

                    else
                    {
                        // Update states and variables
                        m_amountCarrying = 0;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if(DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                            Debug.Log("Trying to drop off food but can't!");
                    }
                        
                    break;
                }

                // Consuming food
                case CHARACTER_OBJECTIVE.FOOD_CONSUME:
                {
                    // Update food stockpile
                    if ( !ResourceManager.IsFoodEmpty() )
                    {
                        ResourceManager.ChangeFood(-1);

						m_fullnessLevel = FULLNESS_LEVEL_MAX;
                        // Update states and variables
                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if (DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                            Debug.Log("Consumed food!");
                    }

                    else
                    {
                        // Update states and variables
                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if (DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                            Debug.Log("No food available!");
                    }

                    break;
                }

                // Gathering wood
                case CHARACTER_OBJECTIVE.WOOD_GATHER:
                {
                    // Only gather wood if we are able to
                    if (CanGatherWood())
                    {
                        // Update states and variables
                        m_amountCarrying = (ushort)m_woodGather;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.CARRYING_WOOD;

                        if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                        {
                            Debug.Log("Wood gathered!");
                            Debug.Log("Amount carrying: " + m_amountCarrying);
                        }

						carryingWoodIcon.SetActive(true);
					}

                    else
                    {
                        // Update states and variables
                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if(DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                            Debug.Log("Trying to gather wood but can't!");
                    }          

                    break;
                }

                // Dropping off wood
                case CHARACTER_OBJECTIVE.WOOD_DROPOFF:
                {
                    // Only drop off wood if the character is carrying wood
                    if (m_action_state == CHARACTER_ACTION_STATE.CARRYING_WOOD)
                    {
                        // Update wood stockpile
                        ResourceManager.ChangeWood(m_amountCarrying);

                        // Update states and variables
                        m_amountCarrying = 0;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if(DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                        {
                            Debug.Log("Dropped off wood!");
						    Debug.Log("Amount carrying: " + m_amountCarrying);
                        }

						carryingWoodIcon.SetActive(false);
					}

                    else
                    {
                        // Update states and variables
                        m_amountCarrying = 0;

                        m_objective     = CHARACTER_OBJECTIVE.NONE;
                        m_action_state  = CHARACTER_ACTION_STATE.IDLE;

                        if(DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                            Debug.Log("Trying to drop off wood but can't!");
                    }        

                    break;
                }

                // Going to love tree
                case CHARACTER_OBJECTIVE.MATE:
                {
                    // Can only go to mate if the character isn't carrying something or already mating
                    if (m_action_state != CHARACTER_ACTION_STATE.CARRYING_FOOD ||
                        m_action_state != CHARACTER_ACTION_STATE.CARRYING_WOOD ||
                        m_action_state != CHARACTER_ACTION_STATE.MATING        ||
                        m_action_state != CHARACTER_ACTION_STATE.WAITING)
                    {
                        // Starts waiting to mate if it is eligible
                        if ( CanMate() )
                        {
                            // Add character to love tree
							if( !LoveTreeScript.IsOccupied( Gender ) )
						    {
                            	LoveTreeScript.AddCharacter(this);
							}

                            // Update states and variables
                            m_objective = CHARACTER_OBJECTIVE.MATE;
                            m_action_state = CHARACTER_ACTION_STATE.WAITING;

                            if (DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                            {
                                Debug.Log("Reached love tree!");
                                Debug.Log("Waiting to mate");
                            }
                        }

                        else
                        {
                            // Update states and variables
                            m_objective = CHARACTER_OBJECTIVE.NONE;
                            m_action_state = CHARACTER_ACTION_STATE.IDLE;

                            if (DEBUG_LEVEL.WARNING <= UNITY_DEBUG)
                                Debug.Log("This character can't mate!");
                        }
                    }

                    else
                    {
                        if (DEBUG_LEVEL.ERROR <= UNITY_DEBUG)
                            Debug.Log("Trying to wait for mating but can't!");
                    }

                    break;
                }

			case CHARACTER_OBJECTIVE.KINDLE:
			{
				ResourceManager.ToggleFire();
				break;
			}

                default:
                {
                    if(DEBUG_LEVEL.WARNING <= UNITY_DEBUG)
                        Debug.Log("NOT IMPLEMENTED! D:");

                    break;
                }
            }
        }

        // Move towards target otherwise
        else
        {
            // Move towards the target position based on character speed
            transform.position = Vector3.MoveTowards(transform.position, m_targetPos, (float)m_speed * .1f * Time.deltaTime);
        }
	}

    /// <summary>
    /// Sets the sorting order for the character so it appears in front of other objects
    /// </summary>
	private void SetSortingOrder()
	{
		spriteRenderer.sortingOrder = Mathf.FloorToInt(100 - transform.position.y * 10);
	}

    // Triggers ------------------------------------------------------------------

    /// <summary>
    /// Functionality for when the character collides with a trigger
    /// </summary>
    /// <param name="colliderObj">The trigger we are colliding with</param>
    void OnTriggerEnter2D(Collider2D colliderObj)
    {
        // If colliding with fire
        if (colliderObj.tag == "Fire")
        {
			if(!m_isToastingMarshmallows)
				m_isToastingMarshmallows = true;
        }

        // If colliding with another trigger
        else
        {
            if (DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Triggering a non-fire obj!");
        }
    }

    /// <summary>
    /// Functionality for when the character leaves a trigger
    /// </summary>
    /// <param name="colliderObj">The trigger we are leaving</param>
    void OnTriggerExit2D(Collider2D colliderObj)
    {
        // If leaving fire
        if (colliderObj.tag == "Fire")
        {
            if (m_isToastingMarshmallows)
                m_isToastingMarshmallows = false;

            if (DEBUG_LEVEL.GENERAL <= UNITY_DEBUG)
                Debug.Log("Trigger Exit: Fire!");
        }

        // If leaving another trigger
        else
        {
            if (DEBUG_LEVEL.WARNING <= UNITY_DEBUG)
                Debug.Log("Triggering a non-fire obj!");
        }
    }

	public static void SetSeason(SeasonManager.Season season)
	{
		currSeason = season;
	}
}
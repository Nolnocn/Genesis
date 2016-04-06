using UnityEngine;
using System.Collections;

public class CharacterUIScript : MonoBehaviour 
{
    // Width of the bars
    private const float BAR_WIDTH = 70;
    // Height of the bars
    private const float BAR_HEIGHT = 10;
    // Width of the icons
    private const float ICON_WIDTH = 10;
    // Height of the icons
    private const float ICON_HEIGHT = 10;
    // X-offsets for the bars
    private const float BAR_OFFSET_X = -33.0f;
    // Y-offset for the health bar
    private const float BAR_OFFSET_Y_HEALTH = 79.0f;
    // Y-offset for the fullness
    private const float BAR_OFFSET_Y_FULLNESS = 67.0f;
    // Y-offset for the heat bar
    private const float BAR_OFFSET_Y_HEAT = 55.0f;
    // X-offsets for the icons
    private const float ICON_OFFSET_X = -45.0f;
    // Y-offset for the health icon
    private const float ICON_OFFSET_Y_HEALTH = BAR_OFFSET_Y_HEALTH;
    // Y-offset for the fullness icon
    private const float ICON_OFFSET_Y_FULLNESS = BAR_OFFSET_Y_FULLNESS;
    // Y-offset for the heat icon
    private const float ICON_OFFSET_Y_HEAT = BAR_OFFSET_Y_HEAT;

    // current health of the source character
    private float m_currentHealth;
    // current heat level of the source character
    private float m_currentHeat;
    // current fullness level of the source character
    private float m_currentFullness;

    // Source character that we are using for position / values
    private CharacterScript sourceCharacter;

    // Postion of the healthbar
    private Vector2 m_posHealthBar;
    // Position of the heat bar
    private Vector2 m_posHeatBar;
    // Position of the fullness bar
    private Vector2 m_posFullnessBar; 
    // Position of the health icon
    private Vector2 m_posHealthIcon;
    // Position of the heat icon
    private Vector2 m_posHeatIcon;
    // Position of the fullness icon
    private Vector2 m_posFullnessIcon;

    // size of the bars
    private Vector2 m_barSize;
    // size of the icons
    private Vector2 m_iconSize;

    // Textures
    public Texture2D health_emptyTexture;
    public Texture2D health_fullTexture;
    public Texture2D health_icon;
    public Texture2D heat_emptyTexture;
    public Texture2D heat_fullTexture;
    public Texture2D heat_icon;
    public Texture2D fullness_emptyTexture;
    public Texture2D fullness_fullTexture;
    public Texture2D fullness_icon;

	// Use this for initialization
	void Start () 
    {
        // set bar size
        m_barSize = new Vector2(BAR_WIDTH, BAR_HEIGHT);

        // set icon size
        m_iconSize = new Vector2(ICON_WIDTH, ICON_HEIGHT);
	}
	
	/// <summary>
	/// Updates the values needed for accurate positions / values for the bars
    /// and icons following our source character
	/// </summary>
	void Update () 
    {
        UpdateValues();
	}

    /// <summary>
    /// Updates necessary bar values from the source character
    /// </summary>
    private void UpdateValues()
    {
        if (sourceCharacter != null)
        {
            // Update current values
            m_currentHealth = sourceCharacter.Health;
            m_currentHeat = sourceCharacter.HeatLevel;
            m_currentFullness = sourceCharacter.FullnessLevel;

            // Set our positions to the current position of the source character
            m_posHealthIcon = m_posFullnessIcon = m_posHeatIcon = m_posHealthBar = m_posFullnessBar = m_posHeatBar = (Vector2) (Camera.main.WorldToScreenPoint(sourceCharacter.transform.position));

            // Bar offsets --------------------------------
            m_posHealthBar.x += BAR_OFFSET_X;
            m_posFullnessBar.x += BAR_OFFSET_X;
            m_posHeatBar.x += BAR_OFFSET_X;

            m_posHealthBar.y += BAR_OFFSET_Y_HEALTH;
            m_posFullnessBar.y += BAR_OFFSET_Y_FULLNESS;
            m_posHeatBar.y += BAR_OFFSET_Y_HEAT;

            // Icon offsets -------------------------------
            m_posHealthIcon.x += ICON_OFFSET_X;
            m_posFullnessIcon.x += ICON_OFFSET_X;
            m_posHeatIcon.x += ICON_OFFSET_X;

            m_posHealthIcon.y += ICON_OFFSET_Y_HEALTH;
            m_posFullnessIcon.y += ICON_OFFSET_Y_FULLNESS;
            m_posHeatIcon.y += ICON_OFFSET_Y_HEAT;
        }
    }
    
    /// <summary>
    /// Draws the health, heat, and fullness bars above the source character
    /// </summary>
    private void OnGUI()
    {
        if (sourceCharacter != null)
        {
            // Health bar ---------------------------------------------------------------

            // draw the 'empty' texture
            GUI.BeginGroup(new Rect(m_posHealthBar.x, Screen.height - m_posHealthBar.y, m_barSize.x, m_barSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_barSize.x, m_barSize.y), health_emptyTexture);
            GUI.EndGroup();

            // Calculate amount to fill with 'full' texture
            float xProgHP = m_barSize.x * (m_currentHealth / 100.0f);

            // draw the current amount available
			GUI.BeginGroup(new Rect(m_posHealthBar.x, Screen.height - m_posHealthBar.y, m_barSize.x, m_barSize.y));
			GUI.DrawTexture(new Rect(0, 0, xProgHP, m_barSize.y), health_fullTexture);
            GUI.EndGroup();

            // draw the icon
            GUI.BeginGroup(new Rect(m_posHealthIcon.x, Screen.height - m_posHealthIcon.y, m_iconSize.x, m_iconSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_iconSize.x, m_iconSize.y), health_icon);
            GUI.EndGroup();

            // Fullness ------------------------------------------------------------------

            // draw the 'empty' texture
            GUI.BeginGroup(new Rect(m_posFullnessBar.x, Screen.height - m_posFullnessBar.y, m_barSize.x, m_barSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_barSize.x, m_barSize.y), fullness_emptyTexture);
            GUI.EndGroup();

            // Calculate amount to fill with 'full' texture
            float xProgFull = BAR_WIDTH * (m_currentFullness / 100.0f);

            // draw the current amount available
			GUI.BeginGroup(new Rect(m_posFullnessBar.x, Screen.height - m_posFullnessBar.y, m_barSize.x, m_barSize.y));
            GUI.DrawTexture(new Rect(0, 0, xProgFull, m_barSize.y), fullness_fullTexture);
            GUI.EndGroup();

            // draw the icon
            GUI.BeginGroup(new Rect(m_posFullnessIcon.x, Screen.height - m_posFullnessIcon.y, m_iconSize.x, m_iconSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_iconSize.x, m_iconSize.y), fullness_icon);
            GUI.EndGroup();      

            // Heat ------------------------------------------------------------------------
            GUI.BeginGroup(new Rect(m_posHeatBar.x, Screen.height - m_posHeatBar.y, m_barSize.x, m_barSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_barSize.x, m_barSize.y), heat_emptyTexture);
            GUI.EndGroup();

            // Calculate amount to fill with 'full' texture
            float xProgHeat = m_barSize.x * (m_currentHeat / 100.0f);

            // draw the current amount available
			GUI.BeginGroup(new Rect(m_posHeatBar.x, Screen.height - m_posHeatBar.y, m_barSize.x, m_barSize.y));
            GUI.DrawTexture(new Rect(0, 0, xProgHeat, m_barSize.y), heat_fullTexture);
            GUI.EndGroup();

            // draw the icon
            GUI.BeginGroup(new Rect(m_posHeatIcon.x, Screen.height - m_posHeatIcon.y, m_iconSize.x, m_iconSize.y));
            GUI.DrawTexture(new Rect(0, 0, m_iconSize.x, m_iconSize.y), heat_icon);
            GUI.EndGroup();          
        }
    }

    /// <summary>
    /// Updates the character that the UI will appear above
    /// </summary>
    /// <param name="character">The character to follow</param>
    public void UpdateSourceCharacter(CharacterScript character)
    {
        // Update the position of the Character UI to be relative
        sourceCharacter = character;

        if(sourceCharacter != null)
            this.transform.parent = sourceCharacter.transform;
    }
}

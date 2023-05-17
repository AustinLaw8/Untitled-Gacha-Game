using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Given the lines a hold note follows, handles the interactions of tapping, particle effects, and holding
public class HoldNote : MonoBehaviour
// , IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject note;
    [SerializeField] private GameObject above;
    [SerializeField] private GameObject below;

    private float INNER_BUMP = .01f;

    public bool holding;

    private GameObject noteSprite;
    private Note startNote;
    private Note endNote;
    private bool endNoteSpawned;
    private Vector2 bottom;
    
    private ParticleSystem particles;

    private PolygonCollider2D col;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private List<(float time, int lane)> points;
    private List<Vector2> uv;
    private List<Vector3> vertices;
    private List<int> triangles;

    public float timer;
    private float pointsTimer;
    private float fallTime;
    private float lastHoldTime;

    private int start;
    private int end;

    void Awake()
    {
        noteSprite = this.transform.GetChild(0).gameObject;
        endNoteSpawned = false;
        bottom = Vector2.zero;

        col = this.gameObject.GetComponent<PolygonCollider2D>();
        above = GameObject.Find("a");
        below = GameObject.Find("b");

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.MarkDynamic();

        // livePoints = new Queue<(float time, int lane)>();
        points = new List<(float time, int lane)>();
        vertices = new List<Vector3>();

        timer = 0f;
        pointsTimer = 0f;
        fallTime = Lane.DISTANCE / Note.fallSpeed;
        start = 0;
        end = 0;
    }

    void Update()
    {
        if (BeatManager.beatManager.IsPlaying)
        {
            // Checks if it has to delete itself
            // This is sort of a hack to ensure the note is destroyed by checking if the endNote is destroyed
            if (endNoteSpawned && endNote == null)
            {
                Destroy(this.gameObject);
            }
            // While holding, play particles and give points
            if (holding)
            {
                pointsTimer += Time.deltaTime;
                if (pointsTimer > .1f)
                {
                    pointsTimer = 0f;
                    ScoreManager.scoreManager.GiveHoldPoints();
                }
                if (bottom != Vector2.zero)
                {
                    PlayParticles();
                }
            }
            else
            {
                if (particles) particles.Stop();
                pointsTimer = 0f;
            }
            
            // Draws the sprite that represents where the "start" of the hold note is
            if (bottom != Vector2.zero)
            {
                noteSprite.transform.position = new Vector3(bottom.x, bottom.y, -2f);
                noteSprite.SetActive(bottom.y>-5.45f);
            }

            timer += Time.deltaTime;

            // Update mesh and collider
            UpdateAll();
        }
    }

    // Gets particles if necessary, and then plays them at the bottom
    private void PlayParticles()
    {
            if (particles == null) particles = ParticleManager.particleManager.AttachParticlesForHold(this.transform);
            if(!particles.isPlaying) particles.Play();
            particles.transform.position = new Vector3(bottom.x,bottom.y, -3f);
    }

    /** 
     * Interface to set the points the hold note will connect when initialized
     *  Calculates distances between points, lines, and draws the mesh
     */
    public void SetPoints(float diffTime, List<(float, int)> newPoints)
    {
        if (points.Count > 0) return;
        float offset = newPoints[0].Item1;
        newPoints = newPoints.ConvertAll<(float, int)>( delegate ((float timeX, int laneX) x) {
            return (x.timeX - offset, x.laneX);
        });
        startNote = GameObject.Instantiate(note).GetComponent<Note>();
        startNote.transform.position = new Vector3(0f, BeatManager.SPAWN_POINT, -2f);
        startNote.SetLane(newPoints[0].Item2);
        startNote.Bump(diffTime);
        startNote.isHold = this;
        startNote.isEnd = false;

        points = new List<(float time, int lane)>(newPoints);
        vertices = new List<Vector3>(points.Count * 2);
        triangles = new List<int>(points.Count * 3);
        uv = new List<Vector2>(points.Count * 2);
        
        UpdateAll();
    }

    // Returns the particles back to the particle manager if they were acquired
    void OnDestroy()
    {
        if (particles) ParticleManager.particleManager.DetachParticlesForHold(particles);
    }

    /* Mesh and Collider Helpers */

    /**
     * Wrapper that calls all necessary helpers to:
     *  calculate all values needed for the mesh
     *  draw the mesh
     *  draw the collider
     */
    private void UpdateAll()
    {
        CalculateVertices();
        CalculateTris();
        CalculateUV();

        // if (vertices.Count >= 3)
        // {
            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = vertices.ConvertAll<Vector3>( x => -Vector3.forward ).ToArray();
            mesh.uv = uv.ToArray();

            meshFilter.mesh = mesh;
            
            SetCollider();
        // }
    }

    /**
     * Uses the vertices to draw the collider
     *  Draws all the way up the right side of the hold note
     *  and then down the left side of the hold note
     */
    private void SetCollider()
    {
        List<Vector2> shapeVertices = new List<Vector2>();
        for(int i = 1; i < vertices.Count; i += 2)
            shapeVertices.Add(vertices[i]);
        for(int i = vertices.Count - 2; i >= 0; i -= 2)
            shapeVertices.Add(vertices[i]);
        GetComponent<PolygonCollider2D>().SetPath(0,shapeVertices.ToArray());
    }

    private void CalculateVertices()
    {
        vertices.Clear();

        // move the right end of the sliding window
        while (end < points.Count && points[end].time <= timer)
        {
            
            // Note newNote = GameObject.Instantiate(note).GetComponent<Note>();
            // newNote.transform.position = new Vector3(0f, BeatManager.SPAWN_POINT, -2f);
            // newNote.SetLane(points[end].Item2);
            end += 1;
        }

        if (holding)
        {
            // Set lastHoldTime. This is needed to know how far the bottom has traveled in case the player releases the hold note
            lastHoldTime = timer;

            if (GetYAtTime(points[start].time) >= BeatManager.PLAY_POINT)
            {
                float y = GetYAtTime(points[start].time);
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane].getX(y) + INNER_BUMP, y, -1f) );
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane + 1].getX(y) - INNER_BUMP, y, -1f) );
                bottom = new Vector2(Lane.LANE_LINES[points[start].lane].getX(BeatManager.PLAY_POINT), BeatManager.PLAY_POINT);
                // lastHoldTime = 0f;
            }
            else
            {
                // move the left end of the sliding window
                while(start < points.Count - 1 && points[start + 1].time + Lane.DISTANCE / Note.fallSpeed <= timer)
                {
                    start++;
                }

                if (start < points.Count - 1)
                {
                    (float time, int lane) lastBelow = points[start];
                    (float time, int lane) firstAbove = points[start + 1];
                    float yBelow = GetYAtTime(lastBelow.time);
                    float yAbove = GetYAtTime(firstAbove.time);
                    float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(yBelow);
                    float xAbove;
                    if (yAbove > BeatManager.SPAWN_POINT)
                    {
                        yAbove = 5f;
                        xAbove = Mathf.Lerp(Lane.LANE_LINES[lastBelow.lane].getX(5f), Lane.LANE_LINES[firstAbove.lane].getX(5f), (timer - lastBelow.time) / (firstAbove.time - lastBelow.time) );
                    }
                    else
                    {
                        xAbove = Lane.LANE_LINES[firstAbove.lane].getX(yAbove);
                    }

                    bottom = Vector2.Lerp(
                        new Vector2(xAbove, BeatManager.PLAY_POINT),
                        new Vector2(xBelow, BeatManager.PLAY_POINT),
                        1 - ((BeatManager.PLAY_POINT - yBelow) / (yAbove - yBelow))
                    );

            // vertices.Add(new Vector3(temp - Lane.TOP_DISTANCE_PER_LANE / 2 + INNER_BUMP, BeatManager.SPAWN_POINT, -1f));
            // vertices.Add(new Vector3(temp + Lane.TOP_DISTANCE_PER_LANE / 2 - INNER_BUMP, BeatManager.SPAWN_POINT, -1f));

                    above.transform.position = new Vector2(xAbove, yAbove);
                    below.transform.position = new Vector2(xBelow, yBelow);
                    //     Debug.Log(
                    //     1 - ((BeatManager.PLAY_POINT - yBelow) / (yAbove - yBelow))
                    // );
                    // lastHoldTime = (timer - fallTime - lastBelow.time) / (firstAbove.time - lastBelow.time);

                    vertices.Add(new Vector3(bottom.x - Lane.BOTTOM_DISTANCE_PER_LANE / 2 + INNER_BUMP, BeatManager.PLAY_POINT, -1f));
                    vertices.Add(new Vector3(bottom.x + Lane.BOTTOM_DISTANCE_PER_LANE / 2 - INNER_BUMP, BeatManager.PLAY_POINT, -1f));
                }
            }
        }
        else
        {
            if (bottom == Vector2.zero)
            {
                float y = GetYAtTime(points[start].time);
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane].getX(y) + INNER_BUMP, y, -1f) );
                vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane + 1].getX(y) - INNER_BUMP, y, -1f) );

                // if (start >= 1 && y <=)
                // {

                // }
                // (float time, int lane) lastBelow = points[start-1];
                // (float time, int lane) firstAbove = points[start];
                // float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(5f);
                // float xAbove = Lane.LANE_LINES[firstAbove.lane].getX(5f);
                // float temp = Mathf.Lerp(xBelow, xAbove, (timer - lastBelow.time) / (firstAbove.time - lastBelow.time) );

                // vertices.Add(new Vector3(temp - Lane.TOP_DISTANCE_PER_LANE / 2 + INNER_BUMP, BeatManager.SPAWN_POINT, -1f));
                // vertices.Add(new Vector3(temp + Lane.TOP_DISTANCE_PER_LANE / 2 - INNER_BUMP, BeatManager.SPAWN_POINT, -1f));
            }
            else
            {
                bottom.y = GetYAtTime(lastHoldTime - fallTime);

                while(start < points.Count - 1 && GetYAtTime(points[start + 1].time) < bottom.y)
                {
                    start++;
                }

                (float time, int lane) lastBelow = points[start];
                (float time, int lane) firstAbove = points[start + 1];
                float yBelow = GetYAtTime(lastBelow.time);
                float yAbove = GetYAtTime(firstAbove.time);
                float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(yBelow);
                float xAbove = Lane.LANE_LINES[firstAbove.lane].getX(yAbove);
                bottom = Vector2.Lerp(
                    new Vector2(xAbove, bottom.y),
                    new Vector2(xBelow, bottom.y),
                    1 - ((bottom.y - yBelow) / (yAbove - yBelow))
                );

                above.transform.position = new Vector2(xAbove, yAbove);
                below.transform.position = new Vector2(xBelow, yBelow);
                // Debug.Log($"{lastHoldTime} {bottom} {(bottom.y - yBelow) / (yAbove - yBelow)}");
                //  {(yAbove - bottom.y) / (yAbove - yBelow)}");
                // bottom.x += (2.5f * Lane.TOP_TO_BOTTOM_DISTANCE_PER_LANE) / fallTime * 2 * Time.deltaTime;

                // if /
                float dist = Mathf.Abs(Lane.LANE_LINES_FOR_OFFSET[points[start].lane].getX(bottom.y) - Lane.LANE_LINES_FOR_OFFSET[points[start].lane + 1].getX(bottom.y));
                vertices.Add( new Vector3(bottom.x - dist / 2 + INNER_BUMP, bottom.y, -1f));
                vertices.Add( new Vector3(bottom.x + dist / 2 - INNER_BUMP, bottom.y, -1f));

                // float y = GetYAtTime(points[start].time);
                // vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane].getX(y) + INNER_BUMP, y, -1f) );
                // vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane + 1].getX(y) - INNER_BUMP, y, -1f) );

                // vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane].Lerp((bottom.y - yBelow) / (yAbove - yBelow)) + INNER_BUMP, bottom.y, -1f) );
                // vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[start].lane + 1].Lerp((bottom.y - yBelow) / (yAbove - yBelow)) - INNER_BUMP, bottom.y, -1f) );

                // if (bottom.y < -5.5f)
                // {
                //     bottom = Vector2.zero;
                // }
            }
        }

        // draw all notes on screen
        for (int i = start + 1; i < end; i++)
        {
            float y = GetYAtTime(points[i].time);
            vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[i].lane].getX(y) + INNER_BUMP, y, -1f) );
            vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[points[i].lane + 1].getX(y) - INNER_BUMP, y, -1f) );
        }

        // ... as well as the first point that goes off screen ...
        if (end < points.Count)
        {
            (float time, int lane) lastBelow = points[end-1];
            (float time, int lane) firstAbove = points[end];
            float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(5f);
            float xAbove = Lane.LANE_LINES[firstAbove.lane].getX(5f);
            float temp = Mathf.Lerp(xBelow, xAbove, (timer - lastBelow.time) / (firstAbove.time - lastBelow.time) );

            vertices.Add(new Vector3(temp - Lane.TOP_DISTANCE_PER_LANE / 2 + INNER_BUMP, BeatManager.SPAWN_POINT, -1f));
            vertices.Add(new Vector3(temp + Lane.TOP_DISTANCE_PER_LANE / 2 - INNER_BUMP, BeatManager.SPAWN_POINT, -1f));
        }

        // .. and spawn the end note if needed
        if (!endNoteSpawned && end == points.Count) 
        {
            endNote = GameObject.Instantiate(note).GetComponent<Note>();
            endNote.transform.position = new Vector3(0f, BeatManager.SPAWN_POINT, -2f);
            endNote.SetLane(points[end - 1].lane);
            endNote.Bump(timer - points[end - 1].time);
            endNote.isHold = this;
            endNote.isEnd = true;
            endNoteSpawned = true;
        }

        // Debug.Log($"{start} {end} {bottom}");
        // Debug.Log("Vertices:");
        // foreach(var v in vertices)
        //     Debug.Log(v);
    }
    // // Very painfully calculates the vertices
    // private void CalculateVertices()
    // {
    //     vertices.Clear();

    //     int i;

    //     float temp, y;
    //     (float time, int lane) x;

    //     // This if block draws the first two vertices
    //     // In pseudocode, the if block looks like:
    //     /*
    //         if not holding
    //             if has not touched yet
    //                 draw vertices as they are defined by points
    //             else
    //                 draw bottom two vertices based on the last calculated bottom of hold note
    //         else
    //             set the bottom of the hold note as the x value of the hold note at the play line
    //             draw bottom two vertices based on this calculated bottom 
    //     */
    //     if (!holding)
    //     {
    //         i = 0;

    //         // If completely missed (hasn't touched the hold note yet)
    //         if (bottom == Vector2.zero)
    //         {
    //             // Basic functionality: go through points, set vertices based on points
    //             while(i < points.Count && points[i].time <= timer)
    //             {
    //                 x = points[i];
    //                 y = GetYAtTime(x.time);
    //                 vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(y) + INNER_BUMP, y, -1f) );
    //                 vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(y) - INNER_BUMP, y, -1f) );
    //                 i++;
    //             }
    //         }
    //         // If hit at some point but then is missing now
    //         else
    //         {
    //             Debug.Log($"befroe: {bottom}");
    //             // If the last point pressed is on screen still, just move it down
    //             if (bottom.y > -5.45f)
    //             {
    //                 i = 0;
    //                 // Find the pair of vertices directly after the bottom, set i to be here
    //                 while(i < points.Count)
    //                 {
    //                     x = points[i];
    //                     y = GetYAtTime(x.time);
    //                     if (bottom.y < y)
    //                         break;
    //                     i++;
    //                 }
                    
    //                 // Draw bottom vertices
    //                 bottom.y = GetYAtTime(lastHoldTime - fallTime);
    //                 if (!Mathf.Approximately(bottom.x, 0f))
    //                 {
    //                     float delta = Lane.BOTTOM_DISTANCE_PER_LANE * Time.deltaTime;
    //                     bottom.x = bottom.x < 0f ? bottom.x - delta : bottom.x + delta;
    //                 }
    //                 vertices.Add( new Vector3(bottom.x - Lane.BOTTOM_DISTANCE_PER_LANE / 2 + INNER_BUMP, bottom.y, -1f));
    //                 vertices.Add( new Vector3(bottom.x + Lane.BOTTOM_DISTANCE_PER_LANE / 2 - INNER_BUMP, bottom.y, -1f));
    //             }
    //             // else, keep track of the bottom manually 
    //             else
    //             {
    //                 // Find the point where the hold note is at the bottom of the screen (find where y = -5.5f)
    //                 i = -1;
    //                 while(i < points.Count - 1)
    //                 //  && points[i+1].time + 10.5f / Note.fallSpeed <= timer)
    //                 {
    //                     if(GetYAtTime(points[i+1].time) > -5.5f)
    //                         break;
    //                     i++;
    //                 }

    //                 // Calculate the x position of the hold note at this point (y=-5f)
    //                 if (i != -1 && i != points.Count - 1)
    //                 {
    //                     (float time, int lane) lastBelow = points[i];
    //                     (float time, int lane) firstAbove = points[i+1];
    //                     float yBelow = GetYAtTime(lastBelow.time);
    //                     float yAbove = GetYAtTime(firstAbove.time);
    //                     float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(yBelow);
    //                     float xAbove = Lane.LANE_LINES[firstAbove.lane].getX(yAbove);
    //                     Debug.Log($"{i}, ({xBelow}, {yBelow}), ({xAbove}, {yAbove})");
    //                     Vector2 limPoint = Vector2.LerpUnclamped(
    //                         new Vector2(xBelow, yBelow),
    //                         new Vector2(xAbove, yAbove),
    //                         (-5.5f - yBelow) / (yAbove - yBelow)
    //                     );
    //                     bottom = limPoint;
    //                     vertices.Add(new Vector3(limPoint.x - 7.2f/7f + INNER_BUMP, -5.5f, -1f));
    //                     vertices.Add(new Vector3(limPoint.x + 7.2f/7f - INNER_BUMP, -5.5f, -1f));
    //                 }
    //                 i++;
    //             }
    //             Debug.Log($"after:{bottom}");

    //         }
    //     }
    //     else // currently holding
    //     {
    //         // Set lastHoldTime. This is needed to know how far the bottom has traveled in case the player releases the hold note
    //         lastHoldTime = timer;

    //         // Find the point where the hold note is at the play line (find where y = -3.4f)
    //         i = -1;
    //         while(i < points.Count - 1 && points[i+1].time + Lane.DISTANCE / Note.fallSpeed <= timer)
    //         {
    //             i++;
    //         }

    //         // If this point is somewhere in the middle of the hold note
    //         // i.e. the hold note is note completely above the play line
    //         // and also not completely below it
    //         // Calculate the x position of the hold note at this point
    //         if (i != -1 && i != points.Count - 1)
    //         {
    //             (float time, int lane) lastBelow = points[i];
    //             (float time, int lane) firstAbove = points[i+1];
    //             float yBelow = GetYAtTime(lastBelow.time);
    //             float yAbove = GetYAtTime(firstAbove.time);
    //             float xBelow = Lane.LANE_LINES[lastBelow.lane].getX(yBelow);
    //             float xAbove = Lane.LANE_LINES[firstAbove.lane].getX(yAbove);
    //             Vector2 limPoint = Vector2.Lerp(
    //                 new Vector2(xBelow, yBelow),
    //                 new Vector2(xAbove, yAbove),
    //                 (BeatManager.PLAY_POINT - yBelow) / (yAbove - yBelow)
    //             );

    //             bottom = limPoint;
    //             vertices.Add(new Vector3(limPoint.x - Lane.BOTTOM_DISTANCE_PER_LANE / 2 + INNER_BUMP, BeatManager.PLAY_POINT, -1f));
    //             vertices.Add(new Vector3(limPoint.x + Lane.BOTTOM_DISTANCE_PER_LANE / 2 - INNER_BUMP, BeatManager.PLAY_POINT, -1f));
    //         }
    //         i++;
    //     }

    //     // Draw everything else that is on screen ...
    //     while(i < points.Count && points[i].time <= timer)
    //     {
    //         x = points[i];
    //         y = GetYAtTime(x.time);
    //         vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(y) + INNER_BUMP, y, -1f) );
    //         vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(y) - INNER_BUMP, y, -1f) );
    //         i++;
    //     }

    //     // ... as well as the first point that goes off screen ...
    //     if (i < points.Count)
    //     {
    //         x = points[i];
    //         temp = Mathf.Pow((timer - x.time)/fallTime, 2);
    //         y = BeatManager.SPAWN_POINT + temp * Lane.DISTANCE;
    //         vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane].getX(5f) + INNER_BUMP, 5f, -1f) );
    //         vertices.Add( new Vector3(Lane.LANE_LINES_FOR_OFFSET[x.lane + 1].getX(5f) - INNER_BUMP, 5f, -1f) );
    //     }
    // }

    /**
     * Sets triangles
     *  For every set of four vertices that make one parallelogram, 
     *  Creates two triangles between them to actually render a full parallelogram
     */
    private void CalculateTris()
    {
        triangles.Clear();
        for (int i = 0; i < vertices.Count / 2 - 1; i++) {
            triangles.Add(i * 2);
            triangles.Add(i * 2 + 2);
            triangles.Add(i * 2 + 1);
            triangles.Add(i * 2 + 1);
            triangles.Add(i * 2 + 2);
            triangles.Add(i * 2 + 3);
        };
    }

    /**
     * Sets UV coordinates
     *  For every set of four vertices that make one parallelogram,
     *  Sets the bottom two to (0,0) and (1,0), and sets the top two to (0,1), (1,1)
     */
    private void CalculateUV()
    {
        uv.Clear();
        for (int i = 0; i < vertices.Count / 2; i++) {
            if (i % 2 == 0)
            {
                uv.Add(new Vector2(0, 0));
                uv.Add(new Vector2(1, 0));
            }
            else
            {
                uv.Add(new Vector2(0, 1));
                uv.Add(new Vector2(1, 1));
            }
        };
    }

    private float GetYAtTime(float t)
    {
        // float temp = Note.fallSpeed * (timer - t);
        // return t > timer ? BeatManager.SPAWN_POINT + temp : BeatManager.SPAWN_POINT - temp;
        float temp = Mathf.Pow((timer - t)/fallTime, 2) * Lane.DISTANCE;
        return t > timer ? BeatManager.SPAWN_POINT + temp : BeatManager.SPAWN_POINT - temp;
    }
}

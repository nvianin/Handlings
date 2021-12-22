using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class WorldManager : MonoBehaviour
{
    public GameObject worldChunk;
    public int radius = 5;
    public int spacing = 32;
    public List<GameObject> chunks;
    public bool regenerate = false;
    private spiralComputer SpiralComputer;

    // Start is called before the first frame update

    void Awake()
    {
        print("World manager initialized");
        SpiralComputer = new spiralComputer(radius);
        SpiralComputer.Generate();
        if (transform.childCount < radius * radius)
        {
            SpiralComputer.positions.ForEach(p =>
            {
                GameObject newChunk = Instantiate(worldChunk, new Vector3(p.x * spacing, 0, p.y * spacing), Quaternion.Euler(-90, 0, 0));
                newChunk.transform.parent = transform;
                chunks.Add(newChunk);
            });
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (regenerate)
        {
            regenerate = false;
            chunks.ForEach(c =>
            {
                Object.DestroyImmediate(c);
            });

            chunks = new List<GameObject>();
            Awake();
        }
    }


    class spiralComputer
    {
        public int width = 0;
        public Vector2 cursor = Vector2.zero;
        public int desiredWidth;
        public List<Vector2> positions;

        public spiralComputer(int _desiredWidth)
        {
            desiredWidth = _desiredWidth;
        }

        public Vector2 nextStep()
        {
            return cursor;
        }

        public void Generate()
        {
            positions = new List<Vector2>();
            while (width != desiredWidth)
            {
                width++;
                positions.Add(cursor);
                cursor.y += 1;
                while (cursor != topright(width))
                {
                    positions.Add(cursor);
                    cursor.x++;
                }
                while (cursor != botright(width))
                {
                    positions.Add(cursor);
                    cursor.y--;
                }
                while (cursor != botleft(width))
                {
                    positions.Add(cursor);
                    cursor.x--;
                }
                while (cursor != topleft(width))
                {
                    positions.Add(cursor);
                    cursor.y++;
                }
            }
            /* cursor.y++; */
            positions.Add(cursor);
        }

        private Vector2 topright(int w)
        {
            return new Vector2(w, w);
        }
        private Vector2 botright(int w)
        {
            return new Vector2(w, -w);
        }
        private Vector2 botleft(int w)
        {
            return new Vector2(-w, -w);
        }
        private Vector2 topleft(int w)
        {
            return new Vector2(-w, w);
        }
    }
}
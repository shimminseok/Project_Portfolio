using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MapPrefabs", menuName = "ScriptableObject/MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<Map> Maps = new List<Map>();

    public void SetMap(int index)
    {
        TextAsset textAsset = Maps[index].Text;
        string text = textAsset.text;

        var json = JObject.Parse(text);

        JArray jPrefabs = json["map"] as JArray;
        GameObject Parent = new GameObject();
        Parent.transform.parent = PoolManager.Instance.transform;
        Parent.transform.localRotation = Quaternion.Euler(0, 45, 0);
        Parent.transform.localPosition = Parent.transform.localPosition - new Vector3(0, 0.01f, 0);
        Maps[index].MapList.Clear();

        foreach (var item in jPrefabs.Children<JObject>())
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(item.GetValue<string>("path", "")), Parent.transform);
            go.transform.localRotation = Quaternion.Euler(new Vector3(item.GetValue<float>("rx", 0f), item.GetValue<float>("ry", 0f), item.GetValue<float>("rz", 0f)));
            go.transform.localPosition = new Vector3(item.GetValue<float>("x", 0), 0, item.GetValue<float>("z", 0f));
            go.transform.localScale = new Vector3(item.GetValue<float>("sx", 1f), item.GetValue<float>("sx", 1f), item.GetValue<float>("sx", 1f));
            go.SetActive(false);
            go.name = Maps[index].name;
            Maps[index].MapList.Add(go);
        }
        int width = json.GetValue<int>("width", 0);
        int height = json.GetValue<int>("height", 0);
        JArray jArray = json["grid"] as JArray;

        Maps[index].GroundSize = new Vector2(width, height);

        Maps[index].MapNode = new Node[(int)(Maps[index].GroundSize.x), (int)(Maps[index].GroundSize.y)];

        foreach (var j in jArray.Children<JObject>())
        {
            int x = j.GetValue<int>("x", 0);
            int y = j.GetValue<int>("y", 0);
            bool monster = j.GetValue<bool>("monster", false);

            Maps[index].MapNode[x, y] = new Node(x, y, true);
            Maps[index].MapNode[x, y].Moveable = !j.GetValue<bool>("block", true);

            Maps[index].MapNode[x, y].Position = Quaternion.Euler(0, 0, -45f) * new Vector2((-Maps[index].GroundSize.x / 2f) + x + 0.5f, (-Maps[index].GroundSize.y / 2f) + y + 0.5f);
            if (monster)
            {
                Vector3 spawnPoint = new Vector3(Maps[index].MapNode[x, y].Position.x, 0, Maps[index].MapNode[x, y].Position.y);
                if (!Maps[index].monsterSpawnPoint.Contains(spawnPoint))
                    Maps[index].monsterSpawnPoint.Add(spawnPoint);
            }

            if (j.GetValue<bool>("start", false))
                Maps[index].start = Maps[index].MapNode[x, y];

        }
    }
}

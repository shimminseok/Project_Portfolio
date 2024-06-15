using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "MapPrefabs", menuName = "ScriptableObject/MapPrefabs")]
public class MapPrefabs : ScriptableObject
{
    public List<Map> Maps = new List<Map>();

    public void SetMap(int index)
    {
        TextAsset textAsset = Maps[index].text;
        string text = textAsset.text;

        var json = JObject.Parse(text);

        JArray jPrefabs = json["map"] as JArray;
        GameObject Parent = new GameObject();
        Parent.transform.parent = PoolManager.Instance.transform;
        //Parent.transform.localRotation = Quaternion.Euler(0, -45, 0);
        Parent.transform.localPosition = Parent.transform.localPosition - new Vector3(0, 0.01f, 0);
        Maps[index].mapList.Clear();

        foreach (var item in jPrefabs.Children<JObject>())
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(item.GetValue<string>("path", "")), Parent.transform);
            go.transform.localRotation = Quaternion.Euler(new Vector3(item.GetValue<float>("rx", 0f), item.GetValue<float>("ry", 0f), item.GetValue<float>("rz", 0f)));
            go.transform.localPosition = new Vector3(item.GetValue<float>("x", 0), 0, item.GetValue<float>("z", 0f));
            go.transform.localScale = new Vector3(item.GetValue<float>("sx", 1f), item.GetValue<float>("sx", 1f), item.GetValue<float>("sx", 1f));
            go.SetActive(false);
            go.name = Maps[index].name;
            Maps[index].mapList.Add(go);
        }
        int width = json.GetValue<int>("width", 0);
        int height = json.GetValue<int>("height", 0);
        JArray jArray = json["grid"] as JArray;

        Maps[index].mapSize = new Vector2(width, height);

        Maps[index].mapNode = new Node[(int)(Maps[index].mapSize.x), (int)(Maps[index].mapSize.y)];
        Maps[index].monsterSpawnPoint = new List<Vector3>();
        foreach (var j in jArray.Children<JObject>())
        {
            int x = j.GetValue<int>("x", 0);
            int y = j.GetValue<int>("y", 0);
            bool monster = j.GetValue<bool>("monster", false);

            bool walkable = !j.GetValue<bool>("block", true);
            Vector3 worldPos = /*Quaternion.Euler(0, 45f, 0) **/ new Vector3((-Maps[index].mapSize.x / 2f) + x, 0, (-Maps[index].mapSize.y / 2f) + y);

            Node map = new Node(walkable, worldPos, x, y);

            Maps[index].mapNode[x, y] = map;
            if (monster)
            {
                Maps[index].monsterSpawnPoint.Add(worldPos);
            }

            if (j.GetValue<bool>("start", false))
                Maps[index].start = Maps[index].mapNode[x, y];

            if (j.GetValue<bool>("boss", false))
                Maps[index].boss = Maps[index].mapNode[x, y];
        }
    }
}

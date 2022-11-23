using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStack : MonoBehaviour
{
    public GameObject Resource;

    public int addAmount;
    public int removeAmount;

    [HideInInspector] public int StackAmount;
    
    private List<GameObject> ItemStack = new();

    private void Update() {
        StackAmount = ItemStack.Count;

        if (Input.GetKeyDown(KeyCode.U)) {
            AddItem(addAmount);
        }

        if (Input.GetKey(KeyCode.O)) {
            ShowItems(removeAmount);
        }

        if (Input.GetKeyDown(KeyCode.I)) {
            RemoveItem(removeAmount);
        }

        for (int i = 0; i < ItemStack.Count; i++) {
            if(ItemStack[i].transform.position.y < 0) {
                ItemStack[i].GetComponent<Rigidbody>().velocity = new Vector3(0, -.1f, 0);
                ItemStack[i].transform.position = new Vector3(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            }

            if (ItemStack[i].GetComponent<Rigidbody>().velocity.magnitude < .1f) {
                if(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(ItemStack[i].transform.position.x, ItemStack[i].transform.position.z)) > .7f)
                    ItemStack[i].transform.position = new Vector3(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            }
        }
    }

    public void AddItem(int amount) {
        for (int i = 0; i < amount; i++) {
            var tmp = Instantiate(Resource, transform);

            tmp.GetComponent<Rigidbody>().velocity = new Vector3(0, -.1f, 0);

            tmp.transform.position = new Vector3(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            tmp.transform.eulerAngles = new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));

            var size = Random.Range(.7f, 1);
            tmp.transform.localScale = new Vector3(size, size, size);

            ItemStack.Add(tmp);
        }
    }

    public void ShowItems(int amount) {
        if(ItemStack.Count < amount) {
            Debug.LogError("Trying to display more Items than in Stack");
            return;
        }

        for (int i = 0; i < amount; i++) {
            var tmp = ItemStack[ItemStack.Count - (i + 1)];

            var DisplayPos = new Vector3(tmp.transform.position.x, transform.position.y + .6f, tmp.transform.position.z);

            tmp.transform.position = Vector3.MoveTowards(tmp.transform.position, DisplayPos, 2 * Time.deltaTime);
            tmp.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }

    public void RemoveItem(int amount) {
        if (ItemStack.Count < amount) {
            Debug.LogError("Trying to Remove more Items than in Stack");
            return;
        }

        for (int i = 0; i < amount; i++) {
            var tmp = ItemStack[ItemStack.Count - 1];

            ItemStack.Remove(tmp);
            Destroy(tmp);
        }
    }
}
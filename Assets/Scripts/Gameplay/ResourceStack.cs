using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStack : MonoBehaviour {
    [SerializeField] private List<GameObject> ResourcePrefabs = new();

    private readonly List<Rigidbody> ItemStack = new();

    private void Update() {
        for (int i = 0; i < ItemStack.Count; i++) {
            if (ItemStack[i].transform.position.y < 0) {
                ItemStack[i].velocity = new(0, -.1f, 0);
                ItemStack[i].transform.position = new(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            }

            if (ItemStack[i].velocity.magnitude < .1f) {
                if (Vector2.Distance(new(transform.position.x, transform.position.z), new(ItemStack[i].transform.position.x, ItemStack[i].transform.position.z)) > .7f)
                    ItemStack[i].transform.position = new(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            }
        }
    }

    public void ShowItems(int amount) {
        if (ItemStack.Count < amount)
            return;

        for (int i = 0; i < amount; i++) {
            Rigidbody item = ItemStack[^(i + 1)];
            Vector3 DisplayPos = new(item.transform.position.x, transform.position.y + .6f, item.transform.position.z);

            item.transform.position = Vector3.MoveTowards(item.transform.position, DisplayPos, 2 * Time.deltaTime);
            item.velocity = new(0, 0, 0);
        }
    }

    public IEnumerator AddItem(int amount) {
        for (int i = 0; i < amount; i++) {
            Rigidbody item = Instantiate(ResourcePrefabs[Random.Range(0, ResourcePrefabs.Count)], transform).GetComponent<Rigidbody>();

            item.velocity = new(0, -.1f, 0);

            item.transform.position = new(transform.position.x + Random.Range(-.25f, .25f), transform.position.y + 2, transform.position.z + Random.Range(-.25f, .25f));
            item.transform.eulerAngles = new(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));

            float size = Random.Range(.7f, 1);
            item.transform.localScale = new(size, size, size);

            ItemStack.Add(item);

            yield return new WaitForSeconds(.1f);

            GameManager.Instance.AudioManager.PlayAudio("ResourceGet");
        }
    }

    public IEnumerator RemoveItem(int amount) {
        if (ItemStack.Count < amount)
            throw new System.Exception("Trying to Remove more Items than in Stack");

        for (int i = 0; i < amount; i++) {
            Rigidbody item = ItemStack[^1];
            item.velocity = new(0, 10, 0);

            yield return new WaitForSeconds(.3f);

            ItemStack.Remove(item);
            Destroy(item);
        }
    }

    public bool HasResource(int amount) {
        return ItemStack.Count >= amount;
    }
}

// 这是一个只适合本程序A*算法使用的二叉堆，堆顶元素最小
public class BinaryHeap {
    public Vertex[] vertexes; // 二叉堆内容

    private int length; // 当前长度

    // 新建二叉堆，最大长度为size
    public BinaryHeap(int size) {
        vertexes = new Vertex[size + 1]; // items[0]并不会使用，所以这里需要一个额外空间
        length = 0;
    }

    // 插入新元素
    public void Add(Vertex vertex) {
        length++; // 当前二叉堆长度加一

        // 新来元素先插到最后面
        vertexes[length] = vertex;

        // 上浮
        Float(length);
    }

    // 对下标为index的元素进行上浮，在对二叉堆元素的值改变后必须调用，以保持二叉堆的有序
    public void Float(int index) {
        // 上浮
        int parentIndex = index / 2;
        while (parentIndex != 0) {
            // 如果自己比父节点小，则与父节点换位
            if (vertexes[index].f() < vertexes[parentIndex].f()) {
                Swap(ref vertexes[index], ref vertexes[parentIndex]);
                index = parentIndex;
                parentIndex = index / 2;
            }
            else break;
        }
    }

    // 删除并返回堆顶元素
    public Vertex Remove() {
        Vertex removedItem = vertexes[1]; // 记下堆顶元素

        vertexes[1] = vertexes[length]; // 最后一个元素取代堆顶元素

        length--; // 当前二叉堆长度减一

        // 下沉
        int currentIndex = 1;
        int leftChildIndex = currentIndex * 2;
        int rightChildIndex = currentIndex * 2 + 1;
        while (leftChildIndex <= length) {
            if (rightChildIndex <= length) { // 有两个子节点
                if (vertexes[leftChildIndex].f() <= vertexes[rightChildIndex].f() && vertexes[leftChildIndex].f() < vertexes[currentIndex].f()) { // 左子节点最小
                    Swap(ref vertexes[currentIndex], ref vertexes[leftChildIndex]);
                    currentIndex = leftChildIndex;
                } else if (vertexes[rightChildIndex].f() < vertexes[leftChildIndex].f() && vertexes[rightChildIndex].f() < vertexes[currentIndex].f()) { // 右子节点最小
                    Swap(ref vertexes[currentIndex], ref vertexes[rightChildIndex]);
                    currentIndex = rightChildIndex;
                }
                else break; // 自己已经是最小的了
            } else { // 只有一个左子节点
                if (vertexes[currentIndex].f() > vertexes[leftChildIndex].f()) { // 左子节点最小
                    Swap(ref vertexes[currentIndex], ref vertexes[leftChildIndex]);
                }
                break; // 无论如何，已经再不会有子节点了
            }

            leftChildIndex = currentIndex * 2;
            rightChildIndex = currentIndex * 2 + 1;
        }

        return removedItem;
    }

    // 返回代表路径点id为waypointId的顶点下标，如果没有，返回0
    public int FindVertex(Waypoint waypoint) {
        for (int i = 1; i <= length; i++) {
            if (vertexes[i].waypoint == waypoint) {
                return i;
            }
        }

        return 0;
    }

    // 二叉堆是否为空
    public bool IsEmpty() {
        return length == 0;
    }

    // 交换两值
    private void Swap(ref Vertex t1, ref Vertex t2) {
        Vertex temp = t1;
        t1 = t2;
        t2 = temp;
    }
}
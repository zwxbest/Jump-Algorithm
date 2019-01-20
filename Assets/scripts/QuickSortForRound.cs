using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.scripts {
  public  class QuickSortForRound {

        private Dictionary<int, int> roundLengthDic = new Dictionary<int, int>();

        private Dictionary<int, int> roundLengthSumDic = new Dictionary<int, int>();

        private bool isSorted = false;

        private bool isGetRoundLength = false;

        public Dictionary<int,int> getRoundLengthSum() {
            if(isSorted == false) {
                throw new Exception("先排序");
            }
            if(isGetRoundLength == false) {
                roundLengthDic = getRoundLengthDic();
            }
            foreach (var item in roundLengthDic) {
                int thisRoundLength = roundLengthDic[item.Key];
                if (item.Key > 1) {
                    int lastRoundLength = roundLengthSumDic[item.Key - 1];
                    roundLengthSumDic.Add(item.Key, lastRoundLength + thisRoundLength);
                } else {
                    roundLengthSumDic.Add(item.Key, thisRoundLength);
                }
            }
            return roundLengthSumDic;
        }

        public Dictionary<int,int> getRoundLengthDic() {
            isSorted = true;
            roundLengthDic =  roundLengthDic.OrderBy(p => p.Key).ToDictionary(p => p.Key, o => o.Value);
            return roundLengthDic;
        }
        public void sort(int[] arr) {
            quickSort(arr, 0, arr.Length - 1, 1);
            isSorted = true;
        }
        private void quickSort(int[] arr, int l, int r, int round) {
            if (l >= r) {
                return;
            }
            int p = partition(arr, l, r, round);
            quickSort(arr, l, p - 1, round + 1);
            quickSort(arr, p + 1, r, round + 1);
        }

        private int partition(int[] arr, int left, int right, int round) {
            int v = arr[left];
            int partition = left;
            int i = left + 1;
            int max = 0;
            if (roundLengthDic.ContainsKey(round)) {
                max = roundLengthDic[round];
            }
            int thisRoundLength = 0;
            for (; i <= right; i++) {
                thisRoundLength++;
                if (arr[i] < v) {
                    partition++;
                    thisRoundLength++;
                    swap(arr, partition, i);
                }
            }
            if (thisRoundLength > max) {
                if (!roundLengthDic.ContainsKey(round)) {
                    roundLengthDic.Add(round, thisRoundLength);
                } else {
                    roundLengthDic.Remove(round);
                    roundLengthDic.Add(round, thisRoundLength);
                }
            }
            if (left != partition) {
                thisRoundLength++;
                swap(arr, left, partition);
            }
            return partition;
        }

        private void swap(int[] arr, int i, int j) {
            int t = arr[i];
            arr[i] = arr[j];
            arr[j] = t;
        }

    }
}

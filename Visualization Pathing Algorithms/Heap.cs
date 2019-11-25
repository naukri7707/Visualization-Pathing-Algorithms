using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualizationPathingAlgorithms
{
	public struct Heap
	{
		//constructor
		public Heap(int indexs)
		{
			this.Index = new Info[indexs];
			last = 0;
		}

		public ref Info this[int index]
		{
			get => ref Index[index];
		}

		private Info[] Index;
		private int last;

		public int Last { get => last; }


		private void Swap(ref Info lhs, ref Info rhs)
		{
			Info tmp = lhs;
			lhs = rhs;
			rhs = tmp;
		}

		private int Cmp(int lhs, int rhs)
		{

			if (Index[lhs].F > Index[rhs].F) return 1;
			else if (Index[lhs].F < Index[rhs].F) return -1;
			if (Index[lhs].G < Index[rhs].G) return 1;
			else if (Index[lhs].G > Index[rhs].G) return -1;
			return 0;
		}

		public void InsNode(Info ins)
		{
			Index[++last] = ins;
			int Now = last, Base = last >> 1;
			while (Base > 0 && Cmp(Now, Base) == -1)
			{
				Swap(ref Index[Now], ref Index[Base]);
				Now = Base;
				Base >>= 1;
			}
		}

		public void DelNode()
		{
			Swap(ref Index[1], ref Index[last--]);
			int Now = 1, Derived = 2;
			while (Derived <= last)
			{
				if (Cmp(Derived + 1, Derived) == -1 && Derived < last) Derived++;
				if (Cmp(Now, Derived) != 1) break;
				Swap(ref Index[Now], ref Index[Derived]);
				Now = Derived;
				Derived <<= 1;
			}
		}

		public void InsNodeQp(Info ins)
		{
			Index[++last] = ins;
			int Now = last, Base = last >> 1;
			while (Base > 0 && Index[Now].H < Index[Base].H)
			{
				Swap(ref Index[Now], ref Index[Base]);
				Now = Base;
				Base >>= 1;
			}
		}

		public void DelNodeQp()
		{
			Swap(ref Index[1], ref Index[last--]);
			int Now = 1, Derived = 2;
			while (Derived <= last)
			{
				if (Index[Derived + 1].H < Index[Derived].H && Derived < last) Derived++;
				if (Index[Now].H <= Index[Derived].H) break;
				Swap(ref Index[Now], ref Index[Derived]);
				Now = Derived;
				Derived <<= 1;
			}
		}
	}
}
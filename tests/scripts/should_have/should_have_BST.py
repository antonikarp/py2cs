
class BST:
    def __init__(self,v,l=None,r=None):
        self.val = v
        self.left = l
        self.right = r
    def show(self):
        if self.left!=None:
            self.left.show()
        print(self.val)
        if self.right!=None:
            self.right.show()
    def add(self,v):
        if v<self.val:
            ll = BST(v) if self.left==None else self.left.add(v)
            return BST(self.val,ll,self.right)
        elif v>self.val:
            rr = BST(v) if self.right==None else self.right.add(v)
            return BST(self.val,self.left,rr)
        else:
            return self
    def remove_subtree(self,v):
        if v<self.val:
            ll = None if self.left==None else self.left.remove_subtree(v)
            return BST(self.val,ll,self.right)
        elif v>self.val:
            rr = None if self.right==None else self.right.remove_subtree(v)
            return BST(self.val,self.left,rr)
        else:
            return None
    def enumerate(self,n=0):
        i = n
        if self.left!=None:
            for v in self.left.enumerate(i):
                yield v
                i += 1
        yield (i,self.val)
        i += 1 
        if self.right!=None:
            for v in self.right.enumerate(i):
                yield v
                i += 1

forest = []
forest.append(None)
forest.append(BST(50))
forest.append(forest[len(forest)-1].add(25))
forest.append(forest[len(forest)-1].add(25))
forest.append(forest[len(forest)-1].add(15))
forest.append(forest[len(forest)-1].add(14))
forest.append(forest[len(forest)-1].add(17))
forest.append(forest[len(forest)-1].add(0))
forest.append(forest[len(forest)-1].add(50))
forest.append(forest[len(forest)-1].add(30))
forest.append(forest[len(forest)-1].add(75))
forest.append(forest[len(forest)-1].add(65))
forest.append(forest[len(forest)-1].add(70))
forest.append(forest[len(forest)-1].add(100))
forest.append(forest[len(forest)-1].add(15))
forest.append(forest[len(forest)-1].add(100))
forest.append(forest[len(forest)-1].add(100))
forest.append(forest[len(forest)-1].remove_subtree(1))
forest.append(forest[len(forest)-1].remove_subtree(15))
forest.append(forest[len(forest)-1].remove_subtree(77))
forest.append(forest[len(forest)-1].remove_subtree(125))
forest.append(forest[len(forest)-1].remove_subtree(100))
forest.append(forest[len(forest)-1].remove_subtree(50))

print('-----')
for tree in forest:
    if tree!=None:
        tree.show()
    print('-----')

for tree in forest:
    if tree!=None:
        print([v for v in tree.enumerate()])

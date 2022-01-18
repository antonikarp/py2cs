M = [ [1,2,3] ,
      [4,5,6] ,
      [7,8,9] ]
row1 = M[1]
col1 = [ r[1] for r in M ]
diag = [ M[i][i] for i in range(len(M)) ]
all  = [ M[i][j] for i in range(3) for j in range(3) ]
print(row1)
print(col1)
print(diag)
print(all)

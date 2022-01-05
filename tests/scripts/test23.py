# Standalone expressions and side effects.
def foo():
	print("Side effect")
	return 2

# The expression will be evaluated and the side effect will be present.
2 + foo()
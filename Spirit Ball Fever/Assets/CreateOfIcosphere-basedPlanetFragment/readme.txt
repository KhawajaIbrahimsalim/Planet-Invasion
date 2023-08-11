Create of Icosphere-based planet fragment
-----------------------------------------------------------------
The script allows the user to enter the smooth surface degree of the planet and the size of each piece to create an icosphere-based planet.
It is also possible to create an empty game object at the center of each piece and at the position of each vertex to possible the placement of the planet's objects.
+ each vertex is shared by six pieces, so you can examine the duplication so that only one position pivot object exists at one vertex.


How To Use? 
-----------------------------------------------------------------
Add the GeneratePlanet script to the component of the object that will create the planet.
Then press the GeneratePlanet button. That's all.


Inspector Options Description
-----------------------------------------------------------------
- GenerateMapRange : Size of the piece (the higher the number, the smaller the piece)
- PlanetDetatil : Smoothness of the planet's surface (the higher the surface, the smoother)
		(!If the number of PlanetDetails exceeds 5, there are too many computations that can strain the system!)
- PlanetScale : Literally

- UseCenterPivotObject : Creates an object with the central position of each piece
- UseOutlinePivotObject : Creates an object with the vertex position of each piece
- outlinePivotObjectOverlapCorrect : Sets whether duplicate pivot objects are created due to vertex sharing of neighboring pieces
		(Check : Deduplication)

- TextureSetting : Set the texture you want to use
		(Set when you want to use lowpoly texture, set to PlanetVertexColorMaterial)
- Color1/2 : Apply color as a random value between two colors
		(!Available using PlanetVertexColorMaterial!)


-----------------------------------------------------------------
DaehoChoi
happyx1225@gmail.com


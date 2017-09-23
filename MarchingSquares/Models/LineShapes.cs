namespace MarchingSquares.Models
{
	internal enum LineShapes
	{
		Empty = 0,				//  ○----○
								//  |    |
								//  |    |
								//  ○----○

		BottomLeft = 1,			//  ○----○
								//  |    |
								//  |    |
								//  ●----○

		BottomRight = 2,		//  ○----○
								//  |    |
								//  |    |
								//  ○----●

		Bottom = 3,				//  ○----○
								//  |    |
								//  |    |
								//  ●----●

		TopRight = 4,			//  ○----●
								//  |    |
								//  |    |
								//  ○----○

		TopRightBottomLeft = 5, //  ○----●
								//  |    |
								//  |    |
								//  ●----○

		Right = 6,				//  ○----●
								//  |    |
								//  |    |
								//  ○----●

		AllButTopLeft = 7,		//  ○----●
								//  |    |
								//  |    |
								//  ●----●

		TopLeft = 8,			//  ●----○
								//  |    |
								//  |    |
								//  ○----○

		Left = 9,				//  ●----○
								//  |    |
								//  |    |
								//  ●----○

		TopLeftBottomRight = 10,//  ●----○
								//  |    |
								//  |    |
								//  ○----●

		AllButTopRight = 11,	//  ●----○
								//  |    |
								//  |    |
								//  ●----●

		Top = 12,				//  ●----●
								//  |    |
								//  |    |
								//  ○----○

		AllButButtomRight = 13,	//  ●----●
								//  |    |
								//  |    |
								//  ●----○

		AllButButtomLeft = 14,	//  ●----●
								//  |    |
								//  |    |
								//  ○----●

		All = 15,				//  ●----●
								//  |    |
								//  |    |
								//  ●----●
	}
}

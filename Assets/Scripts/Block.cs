// Copyright (C) 2017-2020 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

	/// <summary>
	/// The base class for blocks.
	/// </summary>
	public class Block : TileEntity
	{
	    public BlockType type;
	}
	
	public enum BlockType
	{
		Block1,
		Block2,
		Block3,
		Block4,
		RandomBlock,
		Empty,
		Box,
		Stone,
		Vase,
	}


package Engine;
import java.awt.Dimension;
import java.awt.Graphics2D;
import java.awt.Rectangle;
import java.awt.image.BufferedImage;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.security.InvalidParameterException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.imageio.ImageIO;
import javax.swing.JComponent;

import com.sun.javafx.scene.control.skin.CellSkinBase;

import static Engine.Global.*;
import static org.junit.Assert.assertTrue;

class Level extends JComponent {

	private static final long serialVersionUID = 1L;
	protected String levelName;
	protected int tileSize = 16;
	protected final int CHAR_ENCODING = 16;
	protected int charsPerTile;
	protected final int MAX_SECTOR_SIZE = 64;
	protected List<BufferedImage> allTiles = new ArrayList<>();
	protected List<Sector> allSectors;
	protected int spritesWidePerSector, spritesHighPerSector;
	protected int sectorsWide, sectorsHigh;

	public Level(String inLevelName, String tileSetName, int inTileSize, int inWidth, int inHeight) throws IOException {
		int remainder;
		remainder = inWidth % inTileSize;
		if(remainder != 0) {
			throw new InvalidParameterException("Level width must be divisible by the cell size");
		}
		
		remainder = inHeight % inTileSize;
		if(remainder != 0) {
			throw new InvalidParameterException("Level height must be divisible by the cell size");
		}
		
		levelName = inLevelName;
		
		setSize(inWidth, inHeight);
		
		tileSize = inTileSize;
		
		loadTileSet(tileSetName, "png");
		
		saveLevelConfig(levelName, charsPerTile, inWidth, inHeight);
		
		generateLevel(inWidth, inHeight);
		
		saveLevelFile(inLevelName);
	}
	
	public Level(String inLevelName, String tileSetName, int inTileSize) throws IOException {
		levelName = inLevelName;
		
		tileSize = inTileSize;
		
		loadTileSet(tileSetName, "png");
		int totalTiles = allTiles.size();
		
		List<BufferedImage> allImages = new ArrayList<>();
		int maxSpritesWide = readLevelFile(levelName, totalTiles, allImages);
		
		int width = maxSpritesWide * tileSize;
		int totalImages = allImages.size();
		int maxSpritesHigh = totalImages / maxSpritesWide;
		int height = maxSpritesHigh * tileSize;
		
		setSize(width, height);
		
		saveLevelConfig(levelName, charsPerTile, width, height);
		
		generateLevel(allImages, maxSpritesWide);
	}

	public List<Sprite> getAllSpritesWithinBounds(Rectangle boundingBox) {
		List<Sprite> allSprites = new ArrayList<>();
		
		for(Sector aSector : allSectors) {
			int worldHeight = getHeight();
			
			int x = aSector.getCornerX() * spritesWidePerSector * tileSize;
			int y = worldHeight - ((aSector.getCornerY() + 1) * spritesHighPerSector * tileSize);
			
			int sectorWidth = spritesWidePerSector * tileSize;
			int sectorHeight = spritesHighPerSector * tileSize;
			
			Rectangle toCheck = new Rectangle(x, y, sectorWidth, sectorHeight);
			boolean overlaps = boundingBox.intersects(toCheck);
			if(overlaps == false) {
				continue;
			}
			
			List<Sprite> toAdd = aSector.getAllSprites();
			allSprites.addAll(toAdd);
		}
		
		return allSprites;
	}
	
	public boolean isWithinBounds(int toCheckCenterX, int toCheckCenterY, int toCheckWidth, int toCheckHeight) {
		int width = getWidth();
		int height = getHeight();
		Rectangle levelBox = new Rectangle(0, 0, width, height);
		
		int toCheckCornerX = toCheckCenterX - toCheckWidth / 2;
		int toCheckCornerY = toCheckCenterY - toCheckHeight / 2;
		Rectangle toCheck = new Rectangle(toCheckCornerX, toCheckCornerY, toCheckWidth, toCheckHeight);
		
		boolean withinLevel = levelBox.contains(toCheck);
		
		return withinLevel;
	}
	
	private Sector getSector(int sectorX, int sectorY) {
		Sector dummy = new Sector(sectorX, sectorY);
		int index = allSectors.indexOf(dummy);
		if(index == -1) {
			return null;
		}
		
		Sector toFind = allSectors.get(index);
		return toFind;
	}

	protected void loadTileSet(String tileSetName, String extension) {
		String fileName = "tileset" + tileSetName + "." + extension;
		BufferedImage tileSet = loadImage(fileName);
		int cellsWide = tileSet.getWidth() / tileSize;
		int cellsHigh = tileSet.getHeight() / tileSize;
		for (int y = 0; y < cellsHigh; y++) {
			int offsetY = y * tileSize;
			for (int x = 0; x < cellsWide; x++) {
				int offsetX = x * tileSize;
				allTiles.add(tileSet.getSubimage(offsetX, offsetY, tileSize, tileSize));
			}
		}
	}

	private int readLevelFile(String levelName, int tilesetSize, List<BufferedImage> allImages) throws IOException {
		BufferedReader levelInfo;
		File levelFile = new File(root + "/levels/" + levelName + ".lvl");
		try {
			levelInfo = new BufferedReader(new FileReader(levelFile));
		} catch (IOException e) {
			System.out.println("Level file could not be read");
			levelInfo = null;
			throw e;
		}

		int maxSpritesWide = 0;
		charsPerTile = (int) Math.ceil(Math.log10(tilesetSize)/Math.log10(CHAR_ENCODING));

		String line;
		try {
			line = levelInfo.readLine();
		} catch (IOException e) {
			System.out.println("Level file is empty");
			throw e;
		}
		
		while (line != null) {
			int totalCharsPerLine = line.length();
			int totalSpritesPerLine = totalCharsPerLine / charsPerTile;
			
			if(totalSpritesPerLine > maxSpritesWide) {
				maxSpritesWide = totalSpritesPerLine;
			}
			
			List<BufferedImage> lineImages = decodeLine(line, totalSpritesPerLine);
			allImages.addAll(lineImages);
			try {
				line = levelInfo.readLine();
			} catch (IOException e1) {
				line = null;
			}
		}
		try {
			levelInfo.close();
		} catch (IOException e) {
			System.out.println("Failed to close file");
			throw e;
		}
		
		return maxSpritesWide;
	}
	
	private void saveLevelFile(String levelName) throws IOException {
		BufferedWriter levelInfo;
		File levelFile = new File(root + "/levels/" + levelName + ".lvl");
		try {
			levelInfo = new BufferedWriter(new FileWriter(levelFile));
		} catch (IOException e) {
			System.out.println("Level file could not be written");
			levelInfo = null;
			throw e;
		}

		int width = getWidth();
		int height = getHeight();
		
		int totalLines = height / tileSize;
		int lineLength = width / tileSize;
		
		String allLines = "";
		
		Rectangle boundingBox = new Rectangle(0, 0, width, height);
		List<Sprite> allSprites = getAllSpritesWithinBounds(boundingBox);
		
		Comparator<Sprite> sortIntoRows = new Comparator<Sprite>() {
			@Override
			public int compare(Sprite o1, Sprite o2) {
				int y1 = o1.getCenterY();
				
				int y2 = o2.getCenterY();
				
				int comparison = y2 - y1;
				if(comparison == 0) {
					int x1 = o1.getCenterX();
					
					int x2 = o2.getCenterX();
					
					comparison = x1 - x2;
				}
				
				return comparison;
			}
		};
		Collections.sort(allSprites, sortIntoRows);
		
		for(int i = 0; i < totalLines; i++) {
			List<BufferedImage> lineImages = new ArrayList<>();
			
/*			int x = 0;
			//int y = height - i * tileSize;
			int y = i * tileSize;
			Rectangle boundingBox = new Rectangle(x, y, width, tileSize);
			List<Sprite> lineSprites = getAllSpritesWithinBounds(boundingBox);			

			for(Sprite aSprite : lineSprites) {
				BufferedImage image = aSprite.getCurrentImage();
				lineImages.add(image);
			}*/
			
			for(int j = 0; j < lineLength; j++) {
				int index = lineLength * i + j;
				Sprite aSprite = allSprites.get(index);
				BufferedImage image = aSprite.getCurrentImage();
				
				lineImages.add(image);
			}
	
			String line = encodeLine(lineImages);
			
			if(i < (totalLines - 1)) {
				line += "\n";
			}
			allLines += line;
		}
		try {
			levelInfo.write(allLines);
		} catch (IOException e) {
			System.out.println("Line could not be written");
			throw e;
		}
		
		try {
			levelInfo.close();
		} catch (IOException e) {
			System.out.println("Failed to close file");
			throw e;
		}
	}

	private void saveLevelConfig(String levelName, int charsPerTile, int levelWidth, int levelHeight) {
		File levelConfig = new File(root + "/levels/" + levelName + ".cfg");
		BufferedWriter config = null;
		try {
			config = new BufferedWriter(new FileWriter(levelConfig));
			config.write("Name: " + levelName + "\n");
			config.write("Encoding: " + charsPerTile + "\n");
			config.write("Width: " + levelWidth + "\n");
			config.write("Height: " + levelHeight + "\n");
		} catch (IOException e) {
			System.out.println("Config file could not be written to");
			e.printStackTrace();
		}
		
		try {
			config.close();
		} catch (IOException e) {
			System.out.println("Failed to close file");
			e.printStackTrace();
		}
	}
	
	private List<String> loadLevelConfig(String levelName) {
		File levelConfig = new File(root + "/levels/" + levelName + ".cfg");
		BufferedReader config = null;
		List<String> levelInfo = new ArrayList<>();
		
		String line;
		try {
			line = config.readLine();
			levelInfo.add(line);
			while(line != null) {
				line = config.readLine();
				levelInfo.add(line);
			}
		} catch (IOException e){
			System.out.println("Failed to read file");
			e.printStackTrace();
		}
		
		try {
			config.close();
		} catch (IOException e) {
			System.out.println("Failed to close file");
			e.printStackTrace();
		}
		
		return levelInfo;
	}

	private List<BufferedImage> decodeLine(String line, int totalSpritesPerLine) {
		List<BufferedImage> lineImages = new ArrayList<>();
		for(int i = 0; i < totalSpritesPerLine; i++) {
			BufferedImage tile = getTileFromHexCode(line, charsPerTile, i);
			
			lineImages.add(tile);
		}
		return lineImages;
	}
	
	private String encodeLine(List<BufferedImage> lineImages) {
		String line = "";
		int totalImages = lineImages.size();
		if(totalImages == 0) {
			return line;
		}
		
		for(int i = 0; i < totalImages; i++) {
			BufferedImage image = lineImages.get(i);
			
			String hexCode = getHexCodeFromTile(charsPerTile, image);
			
			line += hexCode;
		}
		
		return line;
	}

	private BufferedImage getTileFromHexCode(String line, int charsPerTile,
			int i) {
		int beginIndex = i * charsPerTile;
		int endIndex = i * charsPerTile + 1;
		
		String code = line.substring(beginIndex, endIndex);
		
		code = "0x" + code;
		int index = Integer.decode(code);
		BufferedImage tile = allTiles.get(index);
		return tile;
	}
	
	private String getHexCodeFromTile(int charsPerTile, BufferedImage image) {
		int i = allTiles.indexOf(image);
		String code = Integer.toHexString(i);
		
		return code;
	}

	protected void generateLevel(List<BufferedImage> allImages, int maxSpritesWide) {
		int totalImages = allImages.size();

		int maxSpritesHigh = (int) Math.floor(totalImages / maxSpritesWide);
		
		Dimension sectorDimensions = getSectorDimension(maxSpritesWide, maxSpritesHigh);
		
		generateSectors(allImages, maxSpritesWide, maxSpritesHigh, sectorDimensions);
		
		int levelWidth = maxSpritesWide * tileSize;
		int levelHeight = maxSpritesHigh * tileSize;
		
		this.setSize(levelWidth, levelHeight);
	}
	
	protected void generateLevel(int width, int height) {
		int maxSpritesWide = width / tileSize;
		int maxSpritesHigh = height / tileSize;
		int totalSprites = maxSpritesWide * maxSpritesHigh;
		
		Dimension sectorDimensions = getSectorDimension(maxSpritesWide, maxSpritesHigh);
		
		List<BufferedImage> allImages = new ArrayList<>();
		BufferedImage tile = allTiles.get(0);
		for(int i = 0; i < totalSprites; i++) {
			allImages.add(tile);
		}
		
		generateSectors(allImages, maxSpritesWide, maxSpritesHigh, sectorDimensions);
		
		this.setSize(width, height);
	}

	private Dimension getSectorDimension(int maxSpritesWide,
			int maxSpritesHigh) {
		int sectorWidth = 1;
		int sectorHeight = 1;
		int widthFactor = sectorWidth;
		int heightFactor = sectorHeight;
		int oldWidthFactor = widthFactor;
		int oldHeightFactor = heightFactor;
		int sectorSize = sectorWidth * sectorHeight;
		while(sectorSize < MAX_SECTOR_SIZE) {
			sectorWidth = oldWidthFactor;
			sectorHeight = oldHeightFactor;
			
			widthFactor++;
			int remainder = maxSpritesWide % widthFactor;
			if(remainder == 0) {
				oldWidthFactor = widthFactor;
			}
			
			heightFactor++;
			remainder = maxSpritesHigh % heightFactor;
			if(remainder == 0) {
				oldHeightFactor = heightFactor;
			}
			
			sectorSize = oldWidthFactor * oldHeightFactor;
		}
		Dimension sectorDimensions = new Dimension(sectorWidth, sectorHeight);
		return sectorDimensions;
	}

	private void generateSectors(List<BufferedImage> allImages,
			int maxSpritesWide, int maxSpritesHigh, Dimension sectorDimensions) {
		spritesWidePerSector = sectorDimensions.width;
		spritesHighPerSector = sectorDimensions.height;
		
		sectorsWide = maxSpritesWide / spritesWidePerSector;
		sectorsHigh = maxSpritesHigh / spritesHighPerSector;
		
		allSectors = new ArrayList<>();
		
		populateSectorsWithSprites(allImages);
	}

	private void populateSectorsWithSprites(List<BufferedImage> allImages) {
		for(int i = 0; i < allImages.size(); i++) {
			int sectorX = (i % (spritesWidePerSector * sectorsWide)) / spritesWidePerSector;
			int sectorY = sectorsHigh - (i / (spritesHighPerSector * sectorsWide * sectorsHigh)) - 1;
			
			Sector sector = getSector(sectorX, sectorY);
			if(sector == null) {
				sector = new Sector(sectorX, sectorY);
				allSectors.add(sector);
			}
			
			int tilesWide = spritesWidePerSector * sectorsWide;
			int tilesHigh = spritesHighPerSector * sectorsHigh;
			
			int worldHeight = getHeight();
			
			int centerX = (i % tilesWide) * tileSize + tileSize / 2;
			int centerY = worldHeight - (((i / tilesWide) % tilesHigh) * tileSize + tileSize / 2);
			BufferedImage image = allImages.get(i);
			Sprite toAdd = new Sprite(centerX, centerY, image);
			
			sector.addSprite(toAdd);
		}
	}
	
	private class Sector {
		
		protected int cornerX, cornerY;
		protected List<Sprite> allSprites;
		
		public Sector(int inCornerX, int inCornerY) {
			cornerX = inCornerX;
			cornerY = inCornerY;
			allSprites = new ArrayList<>();
		}
		
		public int getCornerX() {
			return cornerX;
		}
		
		public int getCornerY() {
			return cornerY;
		}
		
		public void addSprite(Sprite toAdd) {
			allSprites.add(toAdd);
		}
		
		public List<Sprite> getAllSprites() {
			return allSprites;
		}
		
		@Override
		public boolean equals(Object other) {
			Sector otherSector = (Sector) other;
			
			int otherX = otherSector.getCornerX();
			if(cornerX != otherX) {
				return false;
			}
			
			int otherY = otherSector.getCornerY();
			if(cornerY != otherY) {
				return false;
			}
			
			return true;
		}
	}
}

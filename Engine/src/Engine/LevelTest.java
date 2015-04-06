package Engine;
import static org.junit.Assert.*;
import org.junit.Before;
import org.junit.Test;

public class LevelTest {

	@Before
	public void setUp() throws Exception {
	}

	@Test
	public void Total280Width20() {
		int totalImages = 280;
		int maxSpritesWide = 20;
		int MAX_SECTOR_SIZE = 64;

		int maxSpritesHigh = (int) Math.floor(totalImages / maxSpritesWide);
		assertTrue(maxSpritesHigh == 14);
		
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
		sectorSize = sectorWidth * sectorHeight;
		
		assertTrue(sectorWidth == 5);
		assertTrue(sectorHeight == 7);
		assertTrue(sectorSize == 35);
		
		int sectorsWide = maxSpritesWide / sectorWidth;
		assertTrue(sectorsWide == 4);
		
		int sectorsHigh = maxSpritesHigh / sectorHeight;
		assertTrue(sectorsHigh == 2);
		
		int totalSectors = sectorsWide * sectorsHigh;
		assertTrue(totalSectors == 8);
	}
	
	@Test
	public void Total290Width20() {
		int totalImages = 290;
		int maxSpritesWide = 20;
		int MAX_SECTOR_SIZE = 64;

		int maxSpritesHigh = (int) Math.floor(totalImages / maxSpritesWide);
		assertTrue(maxSpritesHigh == 14);
		
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
		sectorSize = sectorWidth * sectorHeight;
		
		assertTrue(sectorWidth == 5);
		assertTrue(sectorHeight == 7);
		assertTrue(sectorSize == 35);
		
		int sectorsWide = maxSpritesWide / sectorWidth;
		assertTrue(sectorsWide == 4);
		
		int sectorsHigh = maxSpritesHigh / sectorHeight;
		assertTrue(sectorsHigh == 2);
		
		int totalSectors = sectorsWide * sectorsHigh;
		assertTrue(totalSectors == 8);
	}
	
	@Test
	public void Total8000Width100() {
		int totalImages = 8000;
		int maxSpritesWide = 100;
		int MAX_SECTOR_SIZE = 64;

		int maxSpritesHigh = (int) Math.floor(totalImages / maxSpritesWide);
		assertTrue(maxSpritesHigh == 80);
		
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
		sectorSize = sectorWidth * sectorHeight;
		
		assertTrue(sectorWidth == 5);
		assertTrue(sectorHeight == 8);
		assertTrue(sectorSize == 40);
		
		int sectorsWide = maxSpritesWide / sectorWidth;
		assertTrue(sectorsWide == 20);
		
		int sectorsHigh = maxSpritesHigh / sectorHeight;
		assertTrue(sectorsHigh == 10);
		
		int totalSectors = sectorsWide * sectorsHigh;
		assertTrue(totalSectors == 200);
	}
	
	@Test
	public void sectorIndexes() {
		int total = 24;
		int sectorWidth = 3;
		int sectorHeight = 2;
		int sectorsHigh = 2;
		int sectorSize = 6;
		
		int i = 0;
		
		int columnIndex = (i / sectorWidth) % sectorHeight;
		assertTrue(columnIndex == 0);
		
		int sectorIndex = i / (sectorsHigh * sectorSize);
		assertTrue(sectorIndex == 0);
		
		i = 10;
		
		columnIndex = (i / sectorWidth) % sectorHeight;
		assertTrue(columnIndex == 1);
		
		sectorIndex = i / (sectorsHigh * sectorSize);
		assertTrue(sectorIndex == 0);
		
		i = 19;
		
		columnIndex = (i / sectorWidth) % sectorHeight;
		assertTrue(columnIndex == 0);
		
		sectorIndex = i / (sectorsHigh * sectorSize);
		assertTrue(sectorIndex == 1);
		
		i = 15;
		
		columnIndex = (i / sectorWidth) % sectorHeight;
		assertTrue(columnIndex == 1);
		
		sectorIndex = i / (sectorsHigh * sectorSize);
		assertTrue(sectorIndex == 1);
	}
}

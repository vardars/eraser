<?php
require('../scripts/database.php');

$action = $_GET['action'];
$version = $_GET['version'];
if (empty($action) || empty($version) || !eregi('([0-9]+).([0-9]+).([0-9]+).([0-9]+)', $version))
	exit;

header('content-type: application/xml');
echo '<?xml version="1.0"?>
<updateList version="1.0">' . "\n";

//Output the list of mirrors
$query = mysql_query('SELECT * FROM mirrors ORDER By Continent, Country, City');
echo '	<mirrors>
	<mirror location="(automatically decide)">http://downloads.sourceforge.net/eraser/</mirror>' . "\n";
while ($row = mysql_fetch_array($query))
{
	printf('		<mirror location="%s, %s">%s</mirror>' . "\n", $row['City'], $row['Country'],
		$row['URL']);
}
echo '	</mirrors>';

//Prepare the list of updates
$query = mysql_query(sprintf('SELECT * FROM downloads WHERE
	(MinVersion IS NULL AND MaxVersion IS NULL) OR
	(MinVersion IS NULL AND MaxVersion > \'%1$s\') OR
	(MinVersion <= \'%1$s\' AND MaxVersion IS NULL) OR
	(MinVersion <= \'%1$s\' AND MaxVersion > \'%1$s\')
	ORDER BY `Type` ASC', $version));

$lastItemType = null;
while ($row = mysql_fetch_array($query))
{
	if ($row['Type'] != $lastItemType)
	{
		if ($lastItemType !== null)
			printf('	</%s>' . "\n", $lastItemType);
		printf('	<%s>' . "\n", $row['Type']);
		$lastItemType = $row['Type'];
	}

	printf('		<item name="%s" version="%s" publisher="%s" architecture="%s" filesize="%d">%s</item>
', htmlentities($row['Name']), $row['Version'], htmlentities($row['Publisher']), $row['Architecture'],
			$row['Filesize'], htmlentities($row['Link']));
}

echo '</updateList>
';
?>

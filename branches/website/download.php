<?php
require('./scripts/database.php');
if (empty($_GET['id']))
	exit;

//Get the download link associated with the download
$query = mysql_query(sprintf('SELECT Link FROM downloads WHERE DownloadID=%d', intval($_GET['id'])));
if (!$query)
	exit;
if (!($row = mysql_fetch_array($query)))
	exit;

//Register the download
mysql_query(sprintf('INSERT INTO download_statistics (DownloadID) VALUES (%d)', intval($_GET['id'])));

if (eregi('http(s{0,1})://)(.*)', $row['Link']))
	header('location: ' . $row['Link']);
else
	echo file_get_contents('./' . $row['Link']);
?>

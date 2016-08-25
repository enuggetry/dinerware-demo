<?php

include_once "dblib.php";
$module = "channel";

$params = $_GET;
$params['cmd'] = "list";

processCmd($params);


?>

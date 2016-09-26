<?php

$postdata = file_get_contents("php://input");

$myfile = fopen("dinerdata.json", "w") or die("Unable to open file!");
fwrite($myfile, $postdata);
fclose($myfile);

echo '{"returndata":"hello"}';

?>

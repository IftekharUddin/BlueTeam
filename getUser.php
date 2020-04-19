<?php
if (!array_key_exists('uid', $_SERVER)) {
	header('HTTP/1.1 500 Internal Server Booboo');
	header('Content-Type: application/json; charset=UTF-8');
	die(json_encode(array('message' => '$_SERVER did not contain uid variable!')));
} else {
	$data = array('user' => $_SERVER['uid']);
	header('Content-Type: application/json');
	print json_encode($data);
}

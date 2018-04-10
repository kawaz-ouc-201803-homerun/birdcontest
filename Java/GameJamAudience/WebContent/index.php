<!DOCTYPE html>
<html>
<head>
<meta charset="UTF-8">
<title>鳥人間コンテスト</title>
<script type="text/javascript">
    var ipAddress = '<?php echo $_SERVER['REMOTE_ADDR']; ?>';
    var osEnvironment = window.navigator.userAgent.toLowerCase();
</script>
<script type="text/javascript" src="js/jquery-3.3.1.min.js" ></script>
<script type="text/javascript" src="js/script.js" ></script>
</head>
<body>

<h1>鳥人間コンテスト 投票ページ</h1>

<div>
	１回のゲームにつき、投票できるのは１回までです。

	<form id="postform">
		<h2>ニックネーム</h2>
		<input type="text" id="name" placeholder="例：札幌太郎">

		<h2>予想飛距離 (m)</h2>
		<input type="number" id="predict" min="0" max="99999999">

		<button onclick="Post();">投票</button>
	</form>
</div>

</body>
</html>

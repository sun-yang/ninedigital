description:
	7 testcases are developed to test the webapi
test steps:
	1. open Postman or any other REST client and post json data to the url http://ninedigital.azurewebsites.net
	2. use the test json data in testcase*.txt as the request body
	3. send the request
description and expected result:
	testcase 1:
		description: this is a valid json test data
		result: 7 elements should be sent back successfully
	testcase 2:
		description: this is an invalid json test data without "{""}"
		result: following error message should be sent back
		{
			"error": "Could not decode request: JSON parsing failed"
		}
	testcase 3:
		description: this test case didn't define the value of "drm" & "epsiodcount"
		result:	
		{
			"response": []
		}
	testcase 4:
		description: this is a test case removing "drm" && "epsiodcount"
		result:
		{
			"response": []
		}
	testcase 5:
		description: this is a test case removing one of response element "slug"
		result:
		{
			"response": []
		}
	testcse 6:
		description: this is a test case didn't define the value of "slug"
		result:
		{
			"response": []
		}
	testcase 7:
		description: this testcase defined a huge size of json data to test whether the webapi can handle huge data
		reslut: return json response successfully
other tests:
	1. also should test with other http requests like "get", "put" and etc. and the webapi should return http 405 error.
	2. test post request with non-json format body like "text" format, and the webapi should return http 415 error.
	
		
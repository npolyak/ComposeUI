/// ********************************************************************************************************
///
/// Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License").
/// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
/// See the NOTICE file distributed with this work for additional information regarding copyright ownership.
/// Unless required by applicable law or agreed to in writing, software distributed under the License
/// is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and limitations under the License.
/// 
/// ********************************************************************************************************

var grpc = require('@grpc/grpc-js');
var protoLoader = require('@grpc/proto-loader');
var protobufjs = require('protobufjs')

let topicsProtoFilePath = __dirname + "../../../../Core/Abstractions/Topic.proto"
let subscriptionProtoFilePath = __dirname + "../../../Services/CommunicationsServices/GrpcCommunicationsService/SubscriptionsService.proto"
let messagesProtoFilePath = __dirname + "/../../Services/MessagingService/Messages.proto"

let loadParams =
{
    keepCase: true,
    longs: String,
    enums: String,
    default: true,
    oneofs: true
};


var subscriptionPackageDefinition = protoLoader.loadSync(
    [topicsProtoFilePath, subscriptionProtoFilePath, messagesProtoFilePath],
    loadParams
);

let subsrs = grpc.loadPackageDefinition(subscriptionPackageDefinition).subscriptions;

var client = new subsrs.SubscriptionsService("localhost:30051", grpc.credentials.createInsecure());

var call = client.Subscribe({ topic: 'Test', plugin_id: '1234234' });

var messagesRoot = protobufjs.loadSync(messagesProtoFilePath);

const msgType = messagesRoot.lookupType("subscriptions.TestTopicMessage");

function OnData(testMessage) {
    var msg = msgType.decode(testMessage.message.value);
    console.log(msg);
}

call.on('data', OnData);

call.on('end', function () {
    console.log("END");
});

call.on('error', function (e) {
    console.log("Error" + e);
});

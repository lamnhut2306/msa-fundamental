1. Which architecture pattern is appplied in the MSA Handon Project?
- The implement of communication between Order Service, Product Service and Bank service follow Saga Orchestration Architecture Pattern.
2. Why Saga pattern is the choose one?
- the order workflow is a distributed transaction which happended in 3 services (in the requirement). 
Services under the Saga pattern operate more loosely coupled.
A service doesn't depend directly on other service as REST communication. 
-Especially, in the Orchestrator Saga pattern, a service only communicate with the Orchestrator so the operation of service will be more and more loosely coupled. 
Moreover, due to the high loosely coupling, services are able to work "parallelly".
- The order workflow is also a Long Lived Transaction which contain a sequence of many smaller transactions in services. Each service under Saga pattern has a local transaction, 
Together, they complete step by step the order workflow until the end.
- An Orchestrator Saga pattern controls the flow of saga from a single place. The state of the order can be accessed and managed easily.
- Services in An Orchestrator Saga communicate throught message queue systems. It make sure all message will be handle if it is in the queue.
3. Some cons
- The implementation has high complexity which inscrease by the steps and states of the workflow.
- More complexity mean more effort for developers and difficult to debug.
- Bottleneck: all operattions run throught the Orchestration. If the service goes down, the system will also go down.
- Data will be eventually consistency, not suitable for systems that require high consistency.

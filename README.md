## Evolving With The Changing Requirements - Using DDD and Saga Message Pattern

Modeling business requirements and policies is a tricky thing. Especially when those requirements just keep on changing. Sure, we've all written code before to model requirements, but how can we achieve the dream of evolving the code and at the same time be aligned with the business? After all, isn't that the promise of Domain Driven Design?

Business policies are crucial and for us tech people, so is the right way to model them. We want our code to be extensible and to keep up with the changes in business. Time is also a crucial element in modeling business workflows and is implemented in a multitude of ways like timers, scheduled jobs, etc. In this talk, we'll discuss the saga message pattern to see how you can model complex business workflows, and model time as an immutable durable event so you can implement your business policies in such a manner that it truly evolves around your business. Realize the DDD dream.

## Demo

The code example uses [NServiceBus](https://docs.particular.net/nservicebus/) for messaging and the [Saga feature](https://docs.particular.net/nservicebus/sagas/). 

### First Implementation

The [BeforeTimeouts](https://github.com/indualagarsamy/saga-pattern/tree/master/src/BeforeTimeouts) folder contains an example of a Saga that does not use timeouts.  

### Second Implementation

The [CompleteSolutionWithTimeouts](https://github.com/indualagarsamy/saga-pattern/tree/master/src/CompleteSolutionWithTimeouts) folder contains an example of a Saga that uses timeouts.

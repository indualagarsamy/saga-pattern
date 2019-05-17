## Evolving With The Changing Requirements - Using DDD and Messaging

Modeling business requirements and policies is a tricky thing. Especially when those requirements just keep on changing. Sure, we've all written code before to model requirements, but how can we achieve the dream of evolving the code and at the same time be aligned with the business? After all, isn't that the promise of Domain Driven Design?

Business policies are crucial and for us tech people, so is the right way to model them. We want our code to be extensible and to keep up with the changes in business. Time is also a crucial element in modeling business workflows and is implemented in a multitude of ways like timers, scheduled jobs, etc. In this talk, we'll discuss the saga message pattern to see how you can model complex business workflows, and model time as an immutable durable event so you can implement your business policies in such a manner that it truly evolves around your business. Realize the DDD dream.

## Demo

The code example uses [NServiceBus](https://docs.particular.net/nservicebus/) for messaging and the [Saga feature](https://docs.particular.net/nservicebus/sagas/). 

### First Implementation

The [BeforeTimeouts](https://github.com/indualagarsamy/Presentations/tree/master/evolving-with-changing-requirements/src/BeforeTimeouts) folder contains an example of a Saga that does not use timeouts.  

### Second Implementation

The [CompleteSolutionWithTimeouts](https://github.com/indualagarsamy/Presentations/tree/master/evolving-with-changing-requirements/src/CompleteSolutionWithTimeouts) folder contains an example of a Saga that uses timeouts.

## Explore DDD 2018

* [Talk Recording](https://www.youtube.com/watch?v=pk4GrmWtjMk)

* [Slides](https://github.com/indualagarsamy/Presentations/blob/master/evolving-with-changing-requirements/exploreddd-2018/changing-requirements.pdf)


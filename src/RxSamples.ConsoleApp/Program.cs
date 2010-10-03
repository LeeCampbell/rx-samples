using System;
using System.Linq.Expressions;

namespace RxSamples.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
      /* Uncomment the calls to the samples you wan to see run*/
      var intro = new SubjectExamples();
      //RunExample(() => intro.SubjectExample());
      //RunExample(() => intro.ReplaySubjectExample());
      //RunExample(() => intro.AsyncSubjectExample());
      //RunExample(() => intro.BehaviorSubjectExample());

      var extMethodExamples = new StaticAndExtensionMethodsExamples();
      //RunExample(() => extMethodExamples.Empty_is_like_an_empty_completed_ReplaySubject());
      //RunExample(() => extMethodExamples.Return_is_like_a_completed_BehaviourSubject());
      //RunExample(() => extMethodExamples.Never_is_like_a_Subject());
      //RunExample(() => extMethodExamples.Throw_is_like_ReplaySubject_that_throws_when_subsribed_to());
      //RunExample(() => extMethodExamples.BlockingVsNonBlocking());
      //RunExample(() => extMethodExamples.Range_is_like_Create_that_returns_a_range_of_integers());
      //RunExample(() => extMethodExamples.Inveral_example());
      //RunExample(() => extMethodExamples.Interval_for_evenly_spaced_publications());
      //RunExample(() => extMethodExamples.Start_using_Action_overload());
      //RunExample(() => extMethodExamples.Start_using_Func_overload());
      //RunExample(() => extMethodExamples.ToObservable_can_convert_IEnumables());
      //RunExample(() => extMethodExamples.AsObservable_can_protect_subjects_and_hide_underlying_type());
      //RunExample(() => extMethodExamples.Generate_for_generating_streams_with_lambdas());
      //RunExample(() => extMethodExamples.Any_checks_for_existence_and_returns_Observable_of_bool());
      //RunExample(() => extMethodExamples.Any_will_publish_true_once_its_predicate_is_true());
      //RunExample(() => extMethodExamples.Any_will_publish_false_once_completed_is_published_and_no_values_meets_the_predicate());
      //RunExample(() => extMethodExamples.Contains_is_a_more_specific_version_of_any());
      //RunExample(() => extMethodExamples.IsEmpty_returns_once_a_value_or_OnComplete_is_published());
      //RunExample(() => extMethodExamples.Contains_Any_and_IsEmpty_rethrow_errors());
      //RunExample(() => extMethodExamples.First_Is_a_blocking_call());
      //RunExample(() => extMethodExamples.First_will_throw_if_stream_contains_no_elements());
      //RunExample(() => extMethodExamples.Scan_allows_custom_aggregation());
      //RunExample(() => extMethodExamples.Aggergate_can_be_used_to_create_your_own_aggregates());
      //RunExample(() => extMethodExamples.Take_will_complete_the_stream_after_the_given_number_of_publications());
      //RunExample(() => extMethodExamples.Skip_will_ignore_the_first_number_of_publications());
      //RunExample(() => extMethodExamples.DistinctUntilChanged_will_only_publish_values_that_are_different_to_the_last_published_value());
      //RunExample(() => extMethodExamples.BufferWithCount_will_publish_enumerables_when_the_specified_number_of_values_is_published());
      //RunExample(() => extMethodExamples.BufferWithTime_will_publish_enumerables_when_the_specified_timespan_elapses());
      //RunExample(() => extMethodExamples.Delay_will_time_shift_the_stream_by_a_given_timespan());
      //RunExample(() => extMethodExamples.Delay_will_time_shift_the_stream_by_a_given_DateTimeOffset());
      //RunExample(() => extMethodExamples.Delay_will_not_time_shift_OnError());
      //RunExample(() => extMethodExamples.Sample_will_only_publish_one_value_per_given_timespan());
      //RunExample(() => extMethodExamples.Throttle_wont_publish_values_if_they_are_produced_faster_than_the_throttle_period());
      //RunExample(() => extMethodExamples.Throttle_is_only_useful_for_when_time_between_publications_is_variable());

      var lifetimeExample = new LifetimeManagementExamples();
      //RunExample(() => lifetimeExample.Dispose_of_a_subscription_to_unsubscribe());
      //RunExample(() => lifetimeExample.Multiple_subscriptions_to_the_same_interval_are_actually_independent_streams());
      //RunExample(() => lifetimeExample.OnCompleted_signifies_the_end_of_a_stream_and_OnNext_is_ignored());
      //RunExample(() => lifetimeExample.OnError_signifies_the_end_of_a_stream_and_OnNext_is_ignored());

      var flowControl = new FlowControlExamples();
      //RunExample(() => flowControl.Simple_Subscribe_will_rethrow_Errors());
      //RunExample(() => flowControl.Retry_will_retry_on_error_forever());
      //RunExample(() => flowControl.Retry_with_argument_of_2_will_retry_once());
      //RunExample(() => flowControl.OnErrorResumeNext_will_try_another_stream_OnError());
      //RunExample(() => flowControl.Catch_is_the_same_as_OnErrorResumeNext_when_no_error_type_is_specified());
      //RunExample(() => flowControl.Catch_will_try_another_stream_on_a_specific_exception());
      //RunExample(() => flowControl.Catch_will_OnError_if_OnError_not_the_specific_exception());
      //RunExample(() => flowControl.Do_allows_side_effect_free_access_to_streams());
      //RunExample(() => flowControl.Materialize_Dematerialize_and_Do_for_logging());
      //RunExample(() => flowControl.Run_subscribes_to_all_values_and_blocks());

      var combine = new CombiningObservableExamples();
      //RunExample(() => combine.ConcatSample());
      //RunExample(() => combine.AmbExample());
      //RunExample(() => combine.MergeExample());
      //RunExample(() => combine.SelectManyExample());
      //RunExample(() => combine.SelectMany_IsNotCartesian_Example());
      //RunExample(() => combine.SelectMany_on_3_streams_IsNotCartesian_Example());
      //RunExample(() => combine.ZipExample());
      //RunExample(() => combine.CombineLatestExample());
      //RunExample(() => combine.ForkJoinExample());

    }

    public static void RunExample(Expression<Action> expression)
    {
      using(new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine();
        Console.WriteLine("Running sample '{0}' from {1}", MethodName(expression), TypeName(expression));
      }
      
      var sample = expression.Compile();
      sample();

      using (new ConsoleColor(System.ConsoleColor.DarkGray))
      {
        Console.WriteLine("Press any key to quit.");
      }
      Console.ReadKey();
    }

    private static string TypeName(Expression<Action> method)
    {
      var methodCall = method.Body as MethodCallExpression;
      if (methodCall != null)
      {
        return methodCall.Method.DeclaringType.Name;
      }
      return null;
    }
    
    public static string MethodName(Expression<Action> method)
    {
      var methodCall = method.Body as MethodCallExpression;
      if (methodCall != null)
      {
        return methodCall.Method.Name;
      }
      return null;
    }
  }
}

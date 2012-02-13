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
            //RunExample(() => intro.SubjectInvalidUsageExample());
            //RunExample(() => intro.ReplaySubjectExample());
            //RunExample(() => intro.ReplaySubjectBufferExample());
            //RunExample(() => intro.ReplaySubjectWindowExample());
            //RunExample(() => intro.ReplaySubjectInvalidUsageExample());
            //RunExample(() => intro.BehaviorSubjectExample());
            //RunExample(() => intro.BehaviorSubjectExample2());
            //RunExample(() => intro.BehaviorSubjectCompletedExample());
            //RunExample(() => intro.AsyncSubjectExample());

            var extMethodExamples = new StaticAndExtensionMethodsExamples();
            //RunExample(() => extMethodExamples.Empty_is_like_an_empty_completed_ReplaySubject());
            //RunExample(() => extMethodExamples.Return_is_like_a_completed_replay_one_subject());
            //RunExample(() => extMethodExamples.Never_is_like_a_Subject());
            //RunExample(() => extMethodExamples.Throw_is_like_ReplaySubject_that_throws_when_subsribed_to());


            var lifetimeExample = new LifetimeManagementExamples();
            //RunExample(() => lifetimeExample.Dispose_of_a_subscription_to_unsubscribe());
            //RunExample(() => lifetimeExample.SEH_is_not_valid_for_Rx());
            //RunExample(() => lifetimeExample.Handling_OnError());
            //RunExample(() => lifetimeExample.No_GC_of_subscriptions());
            //RunExample(() => lifetimeExample.Multiple_subscriptions_to_the_same_interval_are_actually_independent_streams());
            //RunExample(() => lifetimeExample.OnCompleted_signifies_the_end_of_a_stream_and_OnNext_is_ignored());
            //RunExample(() => lifetimeExample.OnError_signifies_the_end_of_a_stream_and_OnNext_is_ignored());

            var creationExamples = new CreatingObservableExamples();
            //RunExample(() => creationExamples.Blocking_vs_NonBlocking_via_Observable_Create());
            //RunExample(() => creationExamples.NonBlocking_event_driven());
            //RunExample(() => creationExamples.Use_Generate_to_make_Range());
            //RunExample(() => creationExamples.Sync_ReadFile());
            //RunExample(() => creationExamples.APM_ReadFile());
            //RunExample(() => creationExamples.APM_WithRx_FileRead());
            //RunExample(() => creationExamples.ToObservable_from_TaskT());
            //RunExample(() => creationExamples.ToObservable_from_TaskT_late_start());

            var filtering = new FilteringExamples();
            //RunExample(() => filtering.Where());
            //RunExample(() => filtering.Distinct_will_only_serve_a_value_once());
            //RunExample(() => filtering.DistinctUntilChanged_can_serve_the_same_value_more_than_once());
            //RunExample(() => filtering.IgnoreElements());
            //RunExample(() => filtering.Skip());
            //RunExample(() => filtering.Take());
            //RunExample(() => filtering.SkipWhile());
            //RunExample(() => filtering.TakeWhile());
            //RunExample(() => filtering.SkipLast());
            //RunExample(() => filtering.TakeLast());
            //RunExample(() => filtering.SkipUntil());
            //RunExample(() => filtering.TakeUntil());

            var inspection = new Inspection();
            //RunExample(() => inspection.Any_with_a_value());
            //RunExample(() => inspection.Any_without_a_value());
            //RunExample(() => inspection.Any_with_an_error());
            //RunExample(() => inspection.Any_with_a_value_then_error());
            //RunExample(() => inspection.All_with_values());
            //RunExample(() => inspection.All_without_a_value());
            //RunExample(() => inspection.All_with_failing_values());
            //RunExample(() => inspection.All_with_an_error());
            //RunExample(() => inspection.IsEmpty_is_just_Any_false());
            //RunExample(() => inspection.Contains_with_value());
            //RunExample(() => inspection.DefaultIfEmpty_is_a_noop_if_there_are_values());
            //RunExample(() => inspection.DefaultIfEmpty_returns_Default_T_if_empty());
            //RunExample(() => inspection.ElementAt1());
            //RunExample(() => inspection.ElementAt1_is_Skip1Take1());
            //RunExample(() => inspection.ElementAtLengthPlus1());
            //RunExample(() => inspection.ElementAtOrDefault());
            //RunExample(() => inspection.SequenceEqual());

            var aggregation = new AggregationExamples();
            //RunExample(() => aggregation.Count());
            //RunExample(() => aggregation.LongCount());
            //RunExample(() => aggregation.MinAndAverage());
            //RunExample(() => aggregation.Min_on_null_custom_type());
            //RunExample(() => aggregation.Aggregate_as_Min());
            //RunExample(() => aggregation.First_and_BehaviorSubject());
            //RunExample(() => aggregation.Single());
            //RunExample(() => aggregation.SingleOrDefault());
            //RunExample(() => aggregation.Aggregate_with_one_value());
            //RunExample(() => aggregation.Aggregate_as_Sum());
            //RunExample(() => aggregation.Aggregate_with_empty_sequence_throws());
            //RunExample(() => aggregation.Aggregate_seed_with_empty_sequence_returns_seed());
            //RunExample(() => aggregation.Aggregate_as_Median());
            //RunExample(() => aggregation.Scan());
            //RunExample(() => aggregation.Scan_and_Distinct_for_runningMin());

            var transformation = new TransformationExamples();
            //RunExample(() => transformation.Simple_SelectMany_to_expand());
            //RunExample(() => transformation.SelectMany_to_expand_with_multivalues());
            //RunExample(() => transformation.SelectMany_to_char());
            //RunExample(() => transformation.SelectMany_to_char_with_filter());
            //RunExample(() => transformation.SelectMany_to_make_where());
            //RunExample(() => transformation.SelectMany_to_make_Skip());
            //RunExample(() => transformation.SelectMany_to_make_Take());
            //RunExample(() => transformation.Cast_takes_objects_to_T());
            //RunExample(() => transformation.Cast_fails_on_invalid_cast());
            //RunExample(() => transformation.OfType_takes_objects_to_T_safely());
            //RunExample(() => transformation.TimeStamp());
            //RunExample(() => transformation.TimeInterval());
            //RunExample(() => transformation.TimeInterval_with_StringBuilder());
            //RunExample(() => transformation.TimeInterval_with_StringBuilder_and_OwnThread());
            //RunExample(() => transformation.Materialize_OnCompleted());
            //RunExample(() => transformation.Materialize_OnError());
            //RunExample(() => transformation.Dematerialize_OnCompleted());

            var sideeffects = new SideEffectsExamples();
            //RunExample(() => sideeffects.Simple_count_side_effect());
            //RunExample(() => sideeffects.Simple_count_side_effect_with_2nd_subscription());
            //RunExample(() => sideeffects.Simple_count_side_effect_free());
            //RunExample(() => sideeffects.Do_For_logging_SideEffects());
            //RunExample(() => sideeffects.Do_for_logging_in_a_pipeline());
            //RunExample(() => sideeffects.AsObservable_missing_allowing_sabotage());
            //RunExample(() => sideeffects.AsObservable_protects_encapsulation());
            //RunExample(() => sideeffects.Mutable_elements_cant_be_protected());


            var leavingTheMonad = new LeavingTheMonadExamples();
            //RunExample(() => leavingTheMonad.ForEach_is_Blocking());
            //RunExample(() => leavingTheMonad.Subscribe_is_not_Blocking());
            //RunExample(() => leavingTheMonad.ForEach_with_TryCatch());
            //RunExample(() => leavingTheMonad.Slow_consumer_spike());
            //RunExample(() => leavingTheMonad.Slow_Enumerable_consumer_spike());
            //RunExample(() => leavingTheMonad.ToEnumerable_Subscribes_immediately());
            //RunExample(() => leavingTheMonad.ToEnumerable_caches_and_then_blocks_when_cache_drained());
            //RunExample(() => leavingTheMonad.ToEnumerable_OnError());
            //RunExample(() => leavingTheMonad.ToArray_caches_all_values_then_notifies_the_whole_array());
            //RunExample(() => leavingTheMonad.ToArray_OnError());
            //RunExample(() => leavingTheMonad.ToList_caches_all_values_then_notifies_the_whole_array());
            //RunExample(() => leavingTheMonad.ToList_OnError());
            //RunExample(() => leavingTheMonad.ToDictionary_caches_all_values_then_notifies_the_whole_dictionary());
            //RunExample(() => leavingTheMonad.ToDictionary_OnError());
            //RunExample(() => leavingTheMonad.ToDictionary_throws_on_duplicate_keys());
            //RunExample(() => leavingTheMonad.ToLookup_caches_all_values_then_notifies_the_whole_lookup());
            //RunExample(() => leavingTheMonad.ToLookup_OnError());
            //RunExample(() => leavingTheMonad.Latest_result_will_cache_the_last_Notification());
            //RunExample(() => leavingTheMonad.MostRecent_result_has_a_default_value_and_never_blocks());
            //RunExample(() => leavingTheMonad.Next_result_will_block_on_MoveNext_until_source_notifies());
            //RunExample(() => leavingTheMonad.Next_can_lose_Error());
            //RunExample(() => leavingTheMonad.ToTask_returns_the_last_value_on_completion());
            //RunExample(() => leavingTheMonad.ToTask_OnError());
            //RunExample(() => leavingTheMonad.ToEvent());
            //RunExample(() => leavingTheMonad.ToEvent_OnError());

            var flowControl = new FlowControlExamples();
            //RunExample(() => flowControl.Catch_to_swallow_exceptions());
            //RunExample(() => flowControl.Catch_to_swallow_specific_exceptions());
            //RunExample(() => flowControl.Catch_to_ignore_wrong_exception_type());
            //RunExample(()=>flowControl.Finally_OnCompleted());
            //RunExample(()=>flowControl.Finally_OnError());
            //RunExample(() => flowControl.Finally_dispose_subscription());
            //RunExample(()=>flowControl.Finally_OnError_No_handler());
            //RunExample(()=>flowControl.Custom_Finally_OnComplete());
            //RunExample(()=>flowControl.Custom_Finally_OnError());
            //RunExample(()=>flowControl.Custom_Finally_OnError_No_handler());
            //RunExample(()=>flowControl.Custom_Finally_dispose_Subscription());
            //RunExample(()=>flowControl.Normal_using_keyword());
            //RunExample(() => flowControl.RxUsing_to_bind_resource_lifetime_to_subscription_lifetime());
            //RunExample(() => flowControl.Simple_Subscribe_will_rethrow_Errors());
            //RunExample(() => flowControl.Retry_will_retry_on_error_forever());
            //RunExample(() => flowControl.Retry_will_retry_on_error_forever_MyFix());
            //RunExample(() => flowControl.Retry_with_Schedule_in_create());
            //RunExample(() => flowControl.Retry_with_argument_of_2_will_retry_once());
            //RunExample(() => flowControl.OnErrorResumeNext_will_try_another_stream_OnError());
            //RunExample(() => flowControl.Catch_is_the_same_as_OnErrorResumeNext_when_no_error_type_is_specified());
            //RunExample(() => flowControl.Catch_will_try_another_stream_on_a_specific_exception());
            //RunExample(() => flowControl.Catch_will_OnError_if_OnError_not_the_specific_exception());
            //RunExample(() => flowControl.Do_allows_side_effect_free_access_to_streams());
            //RunExample(() => flowControl.Materialize_Dematerialize_and_Do_for_logging());
            //RunExample(() => flowControl.Run_subscribes_to_all_values_and_blocks());

            var combineExamples = new CombineExamples();
            //RunExample(()=>combineExamples.Concat_continues_with_second_sequence());
            //RunExample(()=>combineExamples.Repeat_continues_with_self());
            //RunExample(()=>combineExamples.StartWith_allows_values_to_be_prepended_to_a_sequence());
            //RunExample(()=>combineExamples.Amb_will_return_values_from_the_first_sequence_to_produce_values());
            //RunExample(()=>combineExamples.Search_With_merge());
            //RunExample(() => combineExamples.CombineLatest());
            //RunExample(() => combineExamples.Zip_example());
            //RunExample(() => combineExamples.Zip_with_skip1());
            //RunExample(() => combineExamples.Basic_AndThenWhen_is_just_zip());
            //RunExample(() => combineExamples.ZipZip());
            //RunExample(() => combineExamples.AndAndThenWhen_is_ZipZip());
            RunExample(() => combineExamples.Composite_AndThenWhen());

            //RunExample(() => extMethodExamples.Defer_to_make_a_blocking_non_blocking_like_Observable_Create());
            //RunExample(() => extMethodExamples.Range_is_like_Create_that_returns_a_range_of_integers());
            //RunExample(() => extMethodExamples.Interval_example());
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


            var scheduleExamples = new SchedulingExamples();
            //RunExample(() => scheduleExamples.Rx_can_still_Deadlock());
            //RunExample(() => scheduleExamples.Scheduling_on_Immediate_is_sequential());
            //RunExample(() => scheduleExamples.Scheduling_on_the_CurrentThread_is_dispatched_to_a_Trampoline());
            //RunExample(() => scheduleExamples.DefaultSchedulingOnObservableCreate());
            //RunExample(() => scheduleExamples.SchedulingOnObservableCreate(Scheduler.ThreadPool, Scheduler.CurrentThread));

            var hotAndCold = new HotAndColdExamples();
            //RunExample(() => hotAndCold.Subject_is_hot_so_starts_regardless_of_subscription());
            //RunExample(() => hotAndCold.Interval_is_cold_so_starts_on_subscription_and_does_not_share_stream());
            //RunExample(() => hotAndCold.Publish_shares_stream_and_Connect_makes_cold_observables_hot());
            //RunExample(() => hotAndCold.Connections_can_be_disposed_and_reconnected());
            //RunExample(() => hotAndCold.Connected_publishes_regardless_of_subscribers());
            //RunExample(() => hotAndCold.RefCount_only_publishes_once_subscribed_to());
            //RunExample(() => hotAndCold.RefCount_is_a_shared_stream_and_unsubscribes_to_underlying_when_no_more_subscribers());
            //RunExample(() => hotAndCold.Prune_will_subscribe_and_return_the_last_value_like_AsyncSubject());
            //RunExample(() => hotAndCold.Replay_wraps_underlying_in_ReplaySubject());
            //RunExample(() => hotAndCold.ReplayOnHotExample());
            //RunExample(() => hotAndCold.AnonQuestion20100917());
            //RunExample(() => hotAndCold.AnonQuestion20100917_using_Inteval());

            var joinsAndWindows = new JoinGroupWindowExamples();
            //RunExample(() => joinsAndWindows.Buffer_With_Count_returns_IObservable_of_IList_of_T());
            //RunExample(() => joinsAndWindows.Buffer_With_Time_returns_IObservable_of_IList_of_T());
            //RunExample(() => joinsAndWindows.Window_With_Count_returns_IObservable_of_IObservable_of_T());
            //RunExample(() => joinsAndWindows.Window_With_Time_returns_IObservable_of_IObservable_of_T());
            //RunExample(() => joinsAndWindows.MyWindow_With_Time_returns_IObservable_of_IObservable_of_T());
            //RunExample(() => joinsAndWindows.Switch_rolls_Windows_back_into_their_source());
            //RunExample(() => joinsAndWindows.Join_with_no_closing_left());
            //RunExample(() => joinsAndWindows.Join_with_immediate_closing_left());
            //RunExample(() => joinsAndWindows.Join_with_non_overlapping_left_windows());
            //RunExample(() => joinsAndWindows.Join_as_CombineLatest());
            //RunExample(() => joinsAndWindows.Actual_CombineLatest());
            //RunExample(() => joinsAndWindows.GroupJoin_with_no_closing_left());
            //RunExample(() => joinsAndWindows.GroupJoin_with_no_closing_right());
            //RunExample(() => joinsAndWindows.GarysOverLappingWindows());
            //RunExample(() => joinsAndWindows.SlightModsToGarysOverLappingWindows());
            //RunExample(() => joinsAndWindows.My_TWAP_or_VWAP_implementation());

            //GroupExample.Run();
        }

        public static void RunExample(Expression<Action> expression)
        {
            using (new ConsoleColor(System.ConsoleColor.DarkGray))
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
                return methodCall.Method.Name.Replace("_", " ");
            }
            return null;
        }
    }
}
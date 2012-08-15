using System;
using System.Linq.Expressions;
using System.Reflection;
using Facts.Toolbox;

namespace Facts
{
	public static class FactExtensions
	{
		public static InstanceT set<InstanceT, MemberT>(
			this InstanceT instance, 
			Expression<Func<InstanceT, MemberT>> accessor, 
			MemberT value)
		{
			return instance.update(accessor, v => value);
		}

		public static InstanceT update<InstanceT, MemberT>(
			this InstanceT instance, 
			Expression<Func<InstanceT, MemberT>> accessor, 
			Func<MemberT, MemberT> change)
		{
			return (InstanceT)update(instance, accessor.Body, i => change((MemberT)i));
		}

		static object update(
			object instance, 
			Expression expression, 
			Func<object, object> c)
		{
			switch (expression.NodeType)
			{
				case ExpressionType.Parameter:
					return c(instance);

				case ExpressionType.MemberAccess:
					var accessor = (MemberExpression) expression;
					var nested = accessor.Expression;
					return update(instance, nested, o => changeInstanceMember(o, accessor, c));

				case ExpressionType.ArrayIndex:
					var arrayIndex = (BinaryExpression) expression;
					var left = arrayIndex.Left;
					return update(instance, left, o => changeArrayElement((Array)o, arrayIndex, c));
					break;

				default:
					throw new Exception("Unsupported expression type {0} to access {1}".format(expression.NodeType, expression));
			}
		}

		static object changeInstanceMember(object instance, MemberExpression accessor, Func<object, object> c) 
		{
			var newInstance = instance.memberwiseClone();

			var member = accessor.Member;
			switch (member.MemberType)
			{
				case MemberTypes.Field:
					{
						var field = (FieldInfo) member;
						var value = field.GetValue(instance);
						var newValue = c(value);
						field.SetValue(newInstance, newValue);
					}
					break;

				case MemberTypes.Property:
					{
						var property = (PropertyInfo)member;
						var value = property.GetValue(instance, NoIndex);
						var newValue = c(value);
						property.SetValue(newInstance, newValue, NoIndex);
					}
					break;

				default:
					throw new Exception("Unsupported member type {0} for member {1} in {2}".format(member.MemberType, member.Name, accessor));
			}

			return newInstance;
		}

		static object changeArrayElement(Array instance, BinaryExpression arrayIndex, Func<object, object> c)
		{
			var newInstance = instance.memberwiseClone();

			var indexExpression = arrayIndex.Right;

			switch (indexExpression.NodeType)
			{
				case ExpressionType.Constant:
					var constant = (ConstantExpression) indexExpression;
					var index = (int)constant.Value;
					var value = instance.GetValue(index);
					var newValue = c(value);
					newInstance.SetValue(newValue, index);
					break;

				default:
					throw new Exception("unsupported index expression {0} for array accessor {1}".format(indexExpression.NodeType, arrayIndex));
			}

			return newInstance;
		}


		static readonly object[] NoIndex = new object[]{};

		public static OT memberwiseClone<OT>(this OT obj)
		{
			return (OT)MWClone(obj);
		}

		static readonly Func<object, object> MWClone = makeMC();

		static Func<object, object> makeMC()
		{
			var m = typeof (object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
			return (Func<object, object>) Delegate.CreateDelegate(typeof (Func<object, object>), m);
		}
	}
}
